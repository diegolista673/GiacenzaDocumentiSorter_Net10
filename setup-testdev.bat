@echo off
cls
echo ========================================
echo Setup User Secrets - TestDev
echo (Ambiente CON LDAP)
echo ========================================
echo.

REM Inizializza User Secrets
echo [1/4] Inizializzazione User Secrets...
dotnet user-secrets init
if errorlevel 1 goto error

echo.
echo [2/4] Configurazione Connection String per TestDev...
echo.
set /p server="Server Test [es: TEST-SQL-SERVER]: "
set /p dbname="Database [default: GIACENZA_SORTER_RM]: "
if "%dbname%"=="" set "dbname=GIACENZA_SORTER_RM"
set /p userid="User ID: "
set /p password="Password: "

set "connString=Server=%server%;Database=%dbname%;User Id=%userid%;Password=%password%;TrustServerCertificate=True;"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "%connString%"
if errorlevel 1 goto error

echo.
echo [3/4] Configurazione Active Directory Service Account...
echo.
set /p adUsername="AD Service Account Username [es: POSTEL\svc_giacenza_test]: "
if "%adUsername%"=="" set "adUsername=POSTEL\svc_giacenza_test"
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "%adUsername%"
if errorlevel 1 goto error

echo.
set /p adPassword="AD Service Account Password: "
if "%adPassword%"=="" (
    echo ATTENZIONE: Password non inserita!
    goto skip_password
)
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "%adPassword%"
if errorlevel 1 goto error

:skip_password
echo.
echo [4/4] Configurazione LDAP Path (opzionale)...
echo.
echo Default: LDAP://test-dc.postel.it (da appsettings.TestDev.json)
set /p customLdap="Inserisci custom LDAP path (o premi ENTER per usare default): "
if not "%customLdap%"=="" (
    dotnet user-secrets set "ActiveDirectory:LdapPath" "%customLdap%"
    if errorlevel 1 goto error
)

echo.
echo ========================================
echo Setup TestDev completato!
echo ========================================
echo.
echo Configurazione salvata:
dotnet user-secrets list
echo.
echo Per avviare l'applicazione:
echo   run-testdev.bat
echo.
echo NOTA: In TestDev, Active Directory è ABILITATO.
echo       Funzionano sia utenti ESTERNI che AD.
echo.
pause
exit /b 0

:error
echo.
echo ERRORE: Setup fallito!
echo Verifica di essere nella directory del progetto.
pause
exit /b 1
