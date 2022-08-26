Remove-Item "package-lock.json"
Remove-Item "@xoutput\api\package-lock.json"
Remove-Item "@xoutput\client\package-lock.json"
Remove-Item "@xoutput\webapp\package-lock.json"

cd '@xoutput\api'
npm i --save
cd '..\client'
npm i --save
cd '..\webapp'
npm i --save
cd '..\..'
npm i --save
