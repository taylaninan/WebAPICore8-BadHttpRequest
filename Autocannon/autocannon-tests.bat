@ECHO OFF

:CSHARPCORE
ECHO "CSHARP (.NET Core 8.x)"

AUTOCANNON -m 'POST' -i autocannon.json --warmup [-c 8 -d 10] -w 8 -c 400 -d 60 -H 'Content-Type':'application/json' -H 'Accept':'application/json' -l http://localhost:5286/api/v1/user_user_login_with_email

:END
PAUSE
