# Building

## css
```
cd /KeyKiosk/KeyKiosk
npm install
npx tailwindcss -i wwwroot/app.css -o wwwroot/app.min.css -m
# or
npx nodemon -e css -i .\wwwroot\app.min.css -x "npx tailwindcss -i wwwroot/app.css -o wwwroot/app.min.css -m"
```

# Install

## Web Service

```//TODO```

## Kiosk Mode

Windows Settings > Accounts > Other Users > Kiosk

Use as digital sign or interactive display

Reset after inactivity 'Never'

Run `ApplyEdgePolicy.bat`