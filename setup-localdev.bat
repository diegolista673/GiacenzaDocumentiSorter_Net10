@echo off
cls
echo ========================================
echo Setup User Secrets - LocalDev
echo (Ambiente SENZA LDAP)
echo ========================================
echo.

REM Inizializza User Secrets
echo [1/2] Inizializzazione User Secrets...
dotnet user-secrets init
if errorlevel 1 goto error

echo.
echo [2/2] Configurazione Connection String per LocalDev...
echo.
echo Database Locale - Opzioni:
echo   1. SQL Express con Windows Authentication (consigliato)
echo   2. SQL Server con user/password
echo   3. Inserisci manualmente
echo.
set /p choice="Scegli opzione (1-3): "

if "%choice%"=="1" (
    set /p dbname="Nome Database [default: GIACENZA_SORTER_RM]: "
    if "%dbname%"=="" set "dbname=GIACENZA_SORTER_RM"
    set "connString=Server=VEVRFL1M031H;Database=%dbname%;Integrated Security=True;TrustServerCertificate=True;"
    echo.
    echo Connection String: %connString%
)

if "%choice%"=="2" (
    set /p server="Server [default: localhost]: "
    if "%server%"=="" set "server=localhost"
    set /p dbname="Database [default: GIACENZA_SORTER_RM]: "
    if "%dbname%"=="" set "dbname=GIACENZA_SORTER_RM"
    set /p userid="User ID: "
    set /p password="Password: "
    set "connString=Server=%server%;Database=%dbname%;User Id=%userid%;Password=%password%;TrustServerCertificate=True;"
)

if "%choice%"=="3" (
    echo.
    echo Inserisci connection string completa:
    set /p connString=""
)

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "%connString%"
if errorlevel 1 goto error

echo.
echo ========================================
echo Setup LocalDev completato!
echo ========================================
echo.
echo Configurazione salvata:
dotnet user-secrets list
echo.
echo Per avviare l'applicazione:
echo   run-localdev.bat
echo.
echo NOTA: In LocalDev, Active Directory è DISABILITATO.
echo       Funzionano solo utenti ESTERNI (password hash nel DB).
echo.
pause
exit /b 0

:error
echo.
echo ERRORE: Setup fallito!
echo Verifica di essere nella directory del progetto.
pause
exit /b 1
