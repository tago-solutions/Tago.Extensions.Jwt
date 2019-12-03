# Jwt Extension

This extension is here to easy the use of JWT integration in .net core applications

It can be configured to:
- validate, authenticate and authorize JWT tokens coming from different sources
- generate unsigned tokens
- sign unsigned token by a predefined kid configuration

## Installation

##### Package Manager
```PM
Install-Package Tago.Extensions.Jwt
```
##### .NET CLI
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
   opts.ConfiguePolicies(Configuration.GetSection("Jwt:Policies"));
});
```


##### Example 2 - Code
```c#
JwtSettings settings = new JwtSettings
{
	DefaultJwt = new JwtConfig
	{
		Audience = "me",
		Issuer = "me",
		ValidateIssuer = true,
	}
};

var cfg1 = new JwtConfig
{
	Audience = "me",
	Issuer = "me",
	SigningSettings = new JwtSigningSettings
	{
		SymmetricKey = new JwtSymmetricKey
		{
			Key = "veryVerySecretKey",
			SecurityAlgorithm = "HS256",
		}
	}
};

//token validators
TokenValidator validator1 = new TokenValidator();
validator1.Fields.Add(new JwtField()
{
	Type = JwtFieldType.Issuer,
	AllowedValues = new string[] { "me" }
});

TokenValidator validator2 = new TokenValidator();
validator2.Fields.Add(new JwtField()
{
	Type = JwtFieldType.Issuer,
	DisallowedValues = new string[] { "me" }
});

cfg1.TokenValidator = validator1;

//set the kid key 
settings.Keys.Add("test", cfg1);



//set policies
JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
string policyKey = "Jwt1"
jwtPolicies.Items.Add(policyKey, validator2);


//add the extension
services.AddJwt(opts => {
	opts.Configure(settings);
	opts.ConfiguePolicies(jwtPolicies);
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
            "Fields": [ // list of fields to validate
              {
                "Type": "Issuer",                
                "AllowedValues": [
                  "me"
                ],
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
          "Fields": [ // list of fields to validate
            {
              "Type": "Issuer",              
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





## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)
