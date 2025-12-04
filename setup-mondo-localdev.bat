@echo off
setlocal enabledelayedexpansion

echo ========================================
echo Setup MondoConnection - LocalDev
echo ========================================
echo.
echo Questo script configura la connection string per il database Mondo
echo in ambiente LocalDev (Azure SQL Database).
echo.

REM Verifica se dotnet ? disponibile
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERRORE: dotnet CLI non trovato. Installare .NET SDK.
    pause
    exit /b 1
)

echo Ambiente: LocalDev (Azure SQL Database Mondo)
echo.

REM Chiedi server Azure SQL
echo Inserisci il server Azure SQL (es: myserver.database.windows.net):
set /p AZURE_SERVER=Server: 

REM Chiedi database name
echo.
echo Inserisci il nome del database Mondo (es: MND_SCATOLE):
set /p DB_NAME=Database: 

REM Chiedi username
echo.
echo Inserisci lo username Azure SQL:
set /p DB_USER=Username: 

REM Chiedi password (nascosta)
echo.
echo Inserisci la password Azure SQL:
set /p DB_PASSWORD=Password: 

REM Costruisci connection string
set CONNECTION_STRING=Server=tcp:%AZURE_SERVER%,1433;Initial Catalog=%DB_NAME%;Persist Security Info=False;User ID=%DB_USER%;Password=%DB_PASSWORD%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

echo.
echo ========================================
echo Configurazione User Secrets
echo ========================================
echo.

REM Configura User Secrets
dotnet user-secrets set "ConnectionStrings:MondoConnection" "%CONNECTION_STRING%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCESS] MondoConnection configurata correttamente!
    echo.
    echo Connection String: Server=tcp:%AZURE_SERVER%,1433;Database=%DB_NAME%;...
    echo.
    echo Per verificare: dotnet user-secrets list
    echo.
) else (
    echo.
    echo [ERRORE] Configurazione fallita. Verificare e riprovare.
    echo.
)

pause
