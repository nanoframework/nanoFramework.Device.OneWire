$vsixPath = "$($env:USERPROFILE)\nanoFramework.Tools.VS2017.Extension.vsix"
(New-Object Net.WebClient).DownloadFile('https://www.myget.org/F/nanoframework-dev/vsix/47973986-ed3c-4b64-ba40-a9da73b44ef7-1.0.1.0.vsix', $vsixPath)
"`"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\VSIXInstaller.exe`" /q /a $vsixPath" | out-file ".\install-vsix.cmd" -Encoding ASCII

'Installing nanoFramework VS extension ...' | Write-Host -ForegroundColor White -NoNewline

& .\install-vsix.cmd > $null

'OK' | Write-Host -ForegroundColor Green
