using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GiacenzaSorterRm.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using GiacenzaSorterRm.Helpers;
using System.ComponentModel.DataAnnotations;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp;
using PdfSharp.Drawing.Layout;
using BarcodeStandard;
using System.Xml.Linq;


namespace GiacenzaSorterRm.Pages.PagesAccettazione
{

    [Authorize(Policy = "NormalizzazioneRequirements")]
    public class CreateModel : PageModel
    {
        private readonly GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterRm.Models.Database.GiacenzaSorterRmTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public SelectList CommesseSL { get; set; } 

        public List<Bancali> BancaliLst { get; set; }

        public BancaliModel BancaliModel { get; set; }

        public DispacciModel DispacciModel { get; private set; } = new DispacciModel();


        [BindProperty(SupportsGet = true)]
        public Bancali Bancali { get; set; }

        public string Ruolo { get; set; }

        public string Utente { get; set; }


        [BindProperty(SupportsGet = true)]
        public List<string> LstDispacci { get; set; } = new List<string>();


        [BindProperty(SupportsGet = true)]
        [StringLength(12,MinimumLength =12, ErrorMessage = "The Dispaccio value must be 12 characters. ")]
        public string Dispaccio { get; set; }



        public async Task<IActionResult> OnGet()
        {
            TempData.Put("elenco", LstDispacci);

            ViewData["Title"] = "Accettazione";

            Ruolo = User.FindFirstValue("Ruolo");
            Utente = User.Identity.Name;

            await InitializePage();

            BancaliModel = new BancaliModel
            {
                BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale)
            };


            return Page();
        }


        public async Task InitializePage()
        {
            int centroID = CentroAppartenenza.SetCentroByUser(User);

            Bancali = new Bancali
            {
                DataAccettazioneBancale = DateTime.Now
            };

            var sel = await _context.Commesses.ToListAsync();

            CommesseSL = new SelectList(sel, "IdCommessa", "Commessa");
             
        }

        public async Task<List<Bancali>> GetListBancali(DateTime? dataLavorazione)
        {
            //elenco tutti i bancali accettati in data odierna 
            if (User.IsInRole("ADMIN"))
            {
                BancaliLst = await _context.Bancalis.Include(s => s.IdCommessaNavigation)
                                                    .Include(s => s.IdCentroArrivoNavigation)
                                                    .Where(x => x.DataAccettazioneBancale.Year == dataLavorazione.Value.Date.Year &&
                                                                x.DataAccettazioneBancale.Month == dataLavorazione.Value.Date.Month &&
                                                                x.DataAccettazioneBancale.Day == dataLavorazione.Value.Date.Day )
                                                    .OrderBy(x=> x.IdBancale)
                                                    .ToListAsync();
            }
            else
            {
                int centroID = CentroAppartenenza.SetCentroByUser(User);

                BancaliLst = await _context.Bancalis.Include(s => s.IdCommessaNavigation)
                                                    .Include(s => s.IdCentroArrivoNavigation)
                                                    .Where(x => x.DataAccettazioneBancale.Year == dataLavorazione.Value.Date.Year &&
                                                                x.DataAccettazioneBancale.Month == dataLavorazione.Value.Date.Month &&
                                                                x.DataAccettazioneBancale.Day == dataLavorazione.Value.Date.Day & x.IdCentroArrivo == Convert.ToInt32(centroID))
                                                    .OrderBy(x => x.IdBancale)
                                                    .ToListAsync();
            }

            return BancaliLst;

        }



        public async Task<IActionResult> OnPostCreaNuovoBancaleAsync()
        {
            if (!ModelState.IsValid)
            {
                await InitializePage();
                BancaliModel = new BancaliModel
                {
                    LastBancale = Bancali.Bancale,
                    BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale)
                };

                return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
            }


            try
            {
                BancaliModel = new BancaliModel();
                                
                string nomeCommessa = "";
                int progressivo = 0;

                Commesse commessa = await _context.Commesses.Where(x => x.IdCommessa == Bancali.IdCommessa).FirstOrDefaultAsync();
                if(commessa != null)
                {
                    //replace spazi vuoti con underscore
                    nomeCommessa = Regex.Replace(nomeCommessa, @"\s+", "_");
                    var prog = await _context.Bancalis.Where(x => x.DataAccettazioneBancale.Date == Bancali.DataAccettazioneBancale.Date).OrderByDescending(i => i.Progressivo).FirstOrDefaultAsync();
                    if(prog != null)
                    {
                        progressivo = (int)prog.Progressivo + 1;
                    }
                    else
                    {
                        progressivo = 1; 
                    }
                   

                    nomeCommessa = commessa.Commessa;
                    Bancali.Bancale = nomeCommessa + " " + Bancali.DataAccettazioneBancale.ToString("yyyyMMdd") + "_" + progressivo.ToString("D3");
                    Bancali.Progressivo = progressivo;

                    using (var stream = CreatePdf(Bancali.Bancale, Bancali.Note))
                    {
                        BancaliModel.PdfBancale = $"data:application/pdf;base64,{Convert.ToBase64String(stream.ToArray())}";
                    }
                    

                    if (ControllaNomeBancale(Bancali.Bancale))
                    {
                        BancaliModel.IsNotConforme = true;
                        BancaliModel.LastBancale = Bancali.Bancale;
                        BancaliModel.BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale);
                        return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
                    }


                    if (!BancaleExists(Bancali.Bancale))
                    {
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            Bancali.OperatoreAccettazione = User.Identity.Name;
                            Bancali.IdOperatoreAccettazione = Convert.ToInt32(User.FindFirstValue("idOperatore"));
                            Bancali.IdCentroArrivo = CentroAppartenenza.SetCentroByUser(User);
                            
                            _context.Bancalis.Add(Bancali);

                            await _context.SaveChangesAsync();

                            LstDispacci = TempData.Get<List<string>>("elenco");

                            if (LstDispacci != null)
                            {
                                foreach (var dispaccio in LstDispacci)
                                {

                                    var BancaliDispacci = new BancaliDispacci();
                                    BancaliDispacci.IdBancale = Bancali.IdBancale;
                                    BancaliDispacci.Bancale = Bancali.Bancale;
                                    BancaliDispacci.Dispaccio = dispaccio;
                                    BancaliDispacci.Operatore = User.Identity.Name;
                                    BancaliDispacci.DataAssociazione = DateTime.Now;
                                    BancaliDispacci.IdCentro = Bancali.IdCentroArrivo;
                                    BancaliDispacci.Centro = CentroAppartenenza.GetCentroLavorazioneByUser(User);

                                    _context.BancaliDispaccis.Add(BancaliDispacci);
                                }

                                await _context.SaveChangesAsync();
                            }


                            transaction.Commit();
                            _logger.LogInformation("Insert bancale accettazione : " + Bancali.Bancale + " - operatore : " + User.Identity.Name);
                        }
                    }
                    else
                    {
                        BancaliModel.IsAccettata = true;
                    }

                    BancaliModel.LastBancale = Bancali.Bancale;
                    BancaliModel.BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale);


                    return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
                }
                else
                {
                    // da implemetare gestione errore
                    await InitializePage();
                    BancaliModel = new BancaliModel
                    {
                        LastBancale = Bancali.Bancale,
                        BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale)
                    };

                    return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
            }
        }



        private MemoryStream CreatePdf(string bancale, string note)
        {
            //create pdf
            using (PdfDocument document = new PdfDocument())
            {
                MemoryStream stream = new MemoryStream();

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Portrait;
                XFont font = new XFont("Arial", 25);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                gfx.DrawString("Bancale", font, XBrushes.Black, new XRect(0, 0-50, page.Width, page.Height), XStringFormats.Center);

                var image = CreaBarcode(bancale);

                gfx.DrawImage(image, new XPoint(image.PointWidth /3, page.Height / 2));

                if (!string.IsNullOrEmpty(note))
                {
                    XFont font1 = new XFont("Arial", 14);
                    var tf = new XTextFormatter(gfx);
                    XStringFormat format = new XStringFormat();
                    format.LineAlignment = XLineAlignment.Near;
                    format.Alignment = XStringAlignment.Near;
                    tf.DrawString("Note:", font, XBrushes.Black, new XRect(20, 570, page.Width, page.Height), format);
                    tf.DrawString(note, font1, XBrushes.Black, new XRect(20, 600, page.Width-70, page.Height), format);
                }

                document.Save(stream,true);

                return stream;                
            }
        }


        public async Task<IActionResult> OnPostPdfBancaleAsync(int IdBancale)
        {
            
            try
            {
                BancaliModel = new BancaliModel();

                var pallet= await _context.Bancalis.Where(x=> x.IdBancale == IdBancale).FirstOrDefaultAsync();  

                if (pallet != null ) {

                    using (var stream = CreatePdf(pallet.Bancale, pallet.Note))
                    {
                        BancaliModel.PdfBancale = $"data:application/pdf;base64,{Convert.ToBase64String(stream.ToArray())}";
                    }
                    
                    BancaliModel.BancaliLst = await GetListBancali(pallet.DataAccettazioneBancale);

                    return Partial("_RiepilogoPdf", BancaliModel);

                }
                else
                {
                    return Partial("_RiepilogoPdf", BancaliModel);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Partial("_RiepilogoPdf", BancaliModel);
            }
        }



        public async Task<IActionResult> OnPostInsertBancaleAsync()
        {
            if (!ModelState.IsValid)
            {
                await InitializePage();
                BancaliModel = new BancaliModel
                {
                    LastBancale = Bancali.Bancale,
                    BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale)
                };

                return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
            }


            try
            {
                BancaliModel = new BancaliModel();

                if (ControllaNomeBancale(Bancali.Bancale))
                {
                    BancaliModel.IsNotConforme = true;
                    BancaliModel.LastBancale = Bancali.Bancale;
                    BancaliModel.BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale);
                    return Partial("_RiepilogoNormalizzateInserite", BancaliModel);
                }


                if (!BancaleExists(Bancali.Bancale))
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        Bancali.OperatoreAccettazione = User.Identity.Name;
                        Bancali.IdOperatoreAccettazione = Convert.ToInt32(User.FindFirstValue("idOperatore"));
                        Bancali.IdCentroArrivo = CentroAppartenenza.SetCentroByUser(User);

                        _context.Bancalis.Add(Bancali);

                        await _context.SaveChangesAsync();

                        LstDispacci = TempData.Get<List<string>>("elenco");

                        if(LstDispacci != null)
                        {
                            foreach (var dispaccio in LstDispacci)
                            {

                                var BancaliDispacci = new BancaliDispacci();
                                BancaliDispacci.IdBancale = Bancali.IdBancale;
                                BancaliDispacci.Bancale = Bancali.Bancale;
                                BancaliDispacci.Dispaccio = dispaccio;
                                BancaliDispacci.Operatore = User.Identity.Name;
                                BancaliDispacci.DataAssociazione = DateTime.Now;
                                BancaliDispacci.IdCentro = Bancali.IdCentroArrivo;
                                BancaliDispacci.Centro = CentroAppartenenza.GetCentroLavorazioneByUser(User);

                                _context.BancaliDispaccis.Add(BancaliDispacci);
                            }

                            await _context.SaveChangesAsync();
                        }


                        transaction.Commit();
                        _logger.LogInformation("Insert bancale accettazione : " + Bancali.Bancale + " - operatore : " + User.Identity.Name);
                    }
                }
                else
                {
                    BancaliModel.IsAccettata = true;
                }

                BancaliModel.LastBancale = Bancali.Bancale;
                BancaliModel.BancaliLst = await GetListBancali(Bancali.DataAccettazioneBancale);


                return Partial("_RiepilogoAccettazioneBancali", BancaliModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Partial("_RiepilogoAccettazioneBancali", BancaliModel );
            }
        }



        /// <summary>
        /// Controlla che il nome scatola non sia ripetuto più volte a causa di un inserimento troppop veloce
        /// conta le occorrenze della parola formata dalle prime 8 lettere all'interno della stringa di input
        /// </summary>
        /// <param name="scatola"></param>
        /// <returns></returns>
        private bool ControllaNomeBancale (string nomeBancale)
        {
            
            if (!string.IsNullOrEmpty(nomeBancale) && (nomeBancale.Length >= 8))
            {
                var part = nomeBancale.Substring(0, 8);
                var count = Regex.Matches(nomeBancale, part).Count;

                if (count > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool BancaleExists(string bancale)
        {
            return _context.Bancalis.Any(e => e.Bancale == bancale);
        }

        public IActionResult OnPostInsertDispacci()
        {
             try
            {
                var temp = TempData.Get<List<string>>("elenco");
                if(temp == null)
                {
                    LstDispacci = new List<string>();
                }
                else
                {
                    
                    LstDispacci = TempData.Get<List<string>>("elenco");
                }

                if (!string.IsNullOrEmpty(Dispaccio))
                {
                    if (!LstDispacci.Contains(Dispaccio.ToString()))
                    {
                        LstDispacci.Add(Dispaccio);
                    }

                }

                TempData.Put("elenco", LstDispacci);
                DispacciModel.ElencoDispacci = TempData.Get<List<string>>("elenco");
                TempData.Keep("elenco");

                return Partial("_RiepilogoDispacci", DispacciModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Partial("_RiepilogoDispacci", DispacciModel);
            }
        }


        private XImage CreaBarcode(string pallet)
        {
            var b = new Barcode();
            b.IncludeLabel = true;
            b.Encode(BarcodeStandard.Type.Code128, pallet, 500, 120);

            MemoryStream ms = new MemoryStream();
            b.SaveImage(ms, SaveTypes.Png);
            
            XImage xbarcode = XImage.FromStream(ms);
            return xbarcode;
        }
    }


}
