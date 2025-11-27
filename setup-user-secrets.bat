@echo off
echo ========================================
echo Setup User Secrets - GiacenzaSorterRm
echo ========================================
echo.

REM Inizializza User Secrets
echo [1/4] Inizializzazione User Secrets...
dotnet user-secrets init
if errorlevel 1 goto error

echo.
echo [2/4] Configurazione Connection String...
set /p connString="Inserisci Connection String (o premi ENTER per default test): "
if "%connString%"=="" set "connString=Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_TEST;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "%connString%"
if errorlevel 1 goto error

echo.
echo [3/4] Configurazione Active Directory Service Account Username...
set /p adUsername="Inserisci AD Service Account Username (es: POSTEL\svc_giacenza): "
if "%adUsername%"=="" set "adUsername=POSTEL\svc_giacenza_test"
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "%adUsername%"
if errorlevel 1 goto error

echo.
echo [4/4] Configurazione Active Directory Service Account Password...
set /p adPassword="Inserisci AD Service Account Password: "
if "%adPassword%"=="" (
    echo ATTENZIONE: Password non inserita. Dovrai configurarla manualmente.
    goto skip_password
)
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "%adPassword%"
if errorlevel 1 goto error

:skip_password
echo.
echo ========================================
echo Setup completato con successo!
echo ========================================
echo.
echo Secrets configurati:
dotnet user-secrets list
echo.
echo Per modificare i secrets:
echo   dotnet user-secrets set "CHIAVE" "VALORE"
echo.
echo Per visualizzare i secrets:
echo   dotnet user-secrets list
echo.
pause
exit /b 0

:error
echo.
echo ERRORE: Setup fallito!
echo Verifica di essere nella directory del progetto.
pause
exit /b 1
