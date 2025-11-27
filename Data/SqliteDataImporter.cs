using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using GiacenzaSorterRm.Data;
using GiacenzaSorterRm.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GiacenzaSorterRm.Data
{
    /// <summary>
    /// Utility per importare dati da CSV esportati da SQL Server in SQLite
    /// </summary>
    public class SqliteDataImporter
    {
        private readonly GiacenzaSorterSqliteContext _context;
        private readonly ILogger _logger;
        private readonly string _exportPath;

        public SqliteDataImporter(GiacenzaSorterSqliteContext context, ILogger logger, string exportPath = @".\Data\Export")
        {
            _context = context;
            _logger = logger;
            _exportPath = exportPath;
        }

        public async Task ImportAllDataAsync()
        {
            _logger.LogInformation("?? Inizio importazione dati in SQLite...");

            try
            {
                // Disabilita foreign keys temporaneamente
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");
                
                // Importa nell'ordine corretto per rispettare le foreign keys
                await ImportCentriLavAsync();
                await ImportStatiAsync();
                await ImportOperatoriAsync();
                await ImportPiattaformeAsync();
                await ImportCommesseAsync();
                await ImportTipologieAsync();
                await ImportContenitoriAsync();
                await ImportTipiNormalizzazioneAsync();
                await ImportCommessaTipologiaContenitoreAsync();
                await ImportBancaliAsync();
                await ImportBancaliDispacciAsync();
                await ImportScatoleAsync();

                // Riabilita foreign keys
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");

                _logger.LogInformation("? Importazione completata con successo!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Errore durante importazione dati");
                throw;
            }
        }

        private async Task ImportCentriLavAsync()
        {
            var csvFile = Path.Combine(_exportPath, "CentriLav.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File CentriLav.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<CentriLav>().ToList();
            
            await _context.CentriLavs.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? CentriLav: {records.Count} righe importate");
        }

        private async Task ImportStatiAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Stati.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Stati.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Stati>().ToList();
            
            await _context.Statis.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Stati: {records.Count} righe importate");
        }

        private async Task ImportOperatoriAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Operatori.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Operatori.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Operatori>().ToList();
            
            await _context.Operatoris.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Operatori: {records.Count} righe importate");
        }

        private async Task ImportPiattaformeAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Piattaforme.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Piattaforme.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Piattaforme>().ToList();
            
            await _context.Piattaformes.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Piattaforme: {records.Count} righe importate");
        }

        private async Task ImportCommesseAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Commesse.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Commesse.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Commesse>().ToList();
            
            await _context.Commesses.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Commesse: {records.Count} righe importate");
        }

        private async Task ImportTipologieAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Tipologie.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Tipologie.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Tipologie>().ToList();
            
            await _context.Tipologies.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Tipologie: {records.Count} righe importate");
        }

        private async Task ImportContenitoriAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Contenitori.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Contenitori.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Contenitori>().ToList();
            
            await _context.Contenitoris.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Contenitori: {records.Count} righe importate");
        }

        private async Task ImportTipiNormalizzazioneAsync()
        {
            var csvFile = Path.Combine(_exportPath, "TipiNormalizzazione.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File TipiNormalizzazione.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<TipiNormalizzazione>().ToList();
            
            await _context.TipiNormalizzaziones.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? TipiNormalizzazione: {records.Count} righe importate");
        }

        private async Task ImportCommessaTipologiaContenitoreAsync()
        {
            var csvFile = Path.Combine(_exportPath, "CommessaTipologiaContenitore.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File CommessaTipologiaContenitore.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<CommessaTipologiaContenitore>().ToList();
            
            await _context.CommessaTipologiaContenitores.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? CommessaTipologiaContenitore: {records.Count} righe importate");
        }

        private async Task ImportBancaliAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Bancali.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Bancali.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Bancali>().ToList();
            
            await _context.Bancalis.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? Bancali: {records.Count} righe importate");
        }

        private async Task ImportBancaliDispacciAsync()
        {
            var csvFile = Path.Combine(_exportPath, "BancaliDispacci.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File BancaliDispacci.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<BancaliDispacci>().ToList();
            
            await _context.BancaliDispaccis.AddRangeAsync(records);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"? BancaliDispacci: {records.Count} righe importate");
        }

        private async Task ImportScatoleAsync()
        {
            var csvFile = Path.Combine(_exportPath, "Scatole.csv");
            if (!File.Exists(csvFile))
            {
                _logger.LogWarning("??  File Scatole.csv non trovato, skip");
                return;
            }

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            
            var records = csv.GetRecords<Scatole>().ToList();
            
            // Importa in batch per performance
            const int batchSize = 1000;
            for (int i = 0; i < records.Count; i += batchSize)
            {
                var batch = records.Skip(i).Take(batchSize).ToList();
                await _context.Scatoles.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"  Scatole: {i + batch.Count}/{records.Count} righe importate");
            }
            
            _logger.LogInformation($"? Scatole: {records.Count} righe importate totali");
        }
    }
}
