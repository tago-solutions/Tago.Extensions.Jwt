# Jwt Extension

This extension lib is intended to easy the use of JWT integration in .net core applications

It can be configured to:
- validate, authenticate and authorize JWT tokens coming from different sources
- generate unsigned tokens
- sign unsigned token by a predefined kid configuration

## Installation

```PM
Install-Package Tago.Extensions.Jwt
```

```c#
dotnet add package Tago.Extensions.Jwt
```

## Usage

#### ConfigureServices

##### Example 1 - Configuration
```c#
services.AddJwt(opts => 
{
   opts.Configure(Configuration.GetSection("Jwt:Settings"));
   opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
   opts.ConfiguePolicies(Configuration.GetSection("Jwt:Policies"));
});
```



#### Configure
```c#
app.UseRouting();
//add this here
app.UseJwt();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});
```


## Configuration

### keys
```javascript
"Jwt": {
    "Settings": {
      "DefaultJwt": {
        "Issuer": "me",
        "Audience": "me",
      },
      "Keys": {
        "test": { // kid key
          "Issuer": "me",
          "Audience": "me",
          "TokenValidator": {
            "Fields": [ // validations fields
              {
                "Type": "Issuer",
                "Name": null,
                "Mandatory": false,
                "AllowedValues": [
                  "me"
                ],
                "DisallowedValues": null
              }
            ]
          },
          "SigningSettings": { // set one of the signing key objects
            "SymmetricKey": {
              "Key": "veryVerySecretKey",
              "SecurityAlgorithm": "HS256"
            },
            "CertificateFile": null,
            "Certificate": null,
            "Jwks": null
          },
          "ValidateLifetime": true,
        }
      }
    },
  }

```


### policies

```javascript
"Jwt": {
     "Policies": {
      "Items": {
        "Jwt1": { //policy key
          "Fields": [ // validations fields
            {
              "Type": "Issuer",
              "Name": null,
              "Mandatory": false,
              "AllowedValues": null,
              "DisallowedValues": [
                "me"
              ]
            }
          ]
        }
      }
    }
  }

```

### validators

```javascript
"Jwt": {    
    "Validators": {
      "test_me": {
        "Fields": [
          {
            "Type": "Issuer",
            "Mandatory": false,
            "MatchType": "Contains",
            "Values": [
              "^(me11|meee)", //starts with one of
              "(me|meee)$", //ends with one of
              "mee"
            ]
          },
          {
            "Type": "Audience",
            "Mandatory": false,
            "MatchType": "Exact",
            "Values": [
              "me"
            ]
          }
        ]
      }
    }
  }
```





## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)