Set-Location ./neon-frontend/
yarn install
yarn build 
Copy-Item  build/*.*  ../Neon.WebApi/wwwroot
Copy-Item  build/static  ../Neon.WebApi/wwwroot/static
Set-Location ../