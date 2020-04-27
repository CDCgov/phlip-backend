# PHLIP Backend
This repository holds the code that are APIs for the project module (everything but the document management) of the PHLIP application. There are other repositories that make up this project, they can be found here:
- Frontend UI - [phlip-frontend](https://github.com/CDCgov/phlip)
- Document Management API - [phlip-doc-management](https://github.com/CDCgov/phlip-doc-management)

## Getting started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites
Below are a list of items that are required and optional for the development environment. Each item is linked to a page about installing it. Really, you only need Docker, the Node/NPM are if you want to run the commands from the package.json file. You're welcome to just look at the package.json file and run the Docker commands instead. All of the code running is done in a Docker container, so the Node and NPM are taken care of there.
 
* [Docker](https://docs.docker.com/engine/installation/ "Installing Docker") - latest stable version
* [.NET Core](https://dotnet.microsoft.com/download) - v3.1.1

### Getting the code
```bash
$ git clone https://github.com/CDCgov/phlip-backend.git
$ cd phlip-backend
```

### Things to know before development
We run the SQL Server in Docker to make things easier. 

### Starting SQL Server
Use the command below to start SQL Server on the default port of 1433.
```bash
$ docker-compose -f docker-compose.sql.yml up
``` 

### Running migrations
Use the command below to run migrations on the SQL Server.
```bash
$ dotnet ef database update
```
(You may get an error about ef not being available/recognized, if that happens, run the following to install the ef tool:)
```bash
$dotnet tool install --global dotnet-ef
```

### Starting the API server
Use the command below to start the API server on port 8000.
```bash
$ dotnet run
```

This application is just an API so there is no frontend. The frontend for this repository is at https://github.com/CDCgov/phlip.git.

### Viewing API documents
This application uses Swagger to document all of the API routes. You can access swagger when the application is running in development mode at http://localhost:8000/swagger.

## Public Domain Standard Notice

This repository constitutes a work of the United States Government and is not subject to domestic copyright protection under 17 USC § 105. This repository is in the public domain within the United States, and copyright and related rights in the work worldwide are waived through the [CC0 1.0 Universal public domain dedication.](https://creativecommons.org/publicdomain/zero/1.0/) All contributions to this repository will be released under the CC0 dedication. By submitting a pull request you are agreeing to comply with this waiver of copyright interest.

## License Standard Notice

This project constitutes a work of the United States Government and is not subject to domestic copyright protection under 17 USC § 105.

The project utilizes code licensed under the terms of the Apache Software License and therefore is licensed under ASL v2 or later.

This program is free software: you can redistribute it and/or modify it under the terms of the Apache Software License version 2, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the Apache Software License for more details.

You should have received a copy of the Apache Software License along with this program. If not, see <http://www.apache.org/licenses/LICENSE-2.0.html>.

## Privacy Standard Notice

This project contains only non-sensitive, publicly available data and information. All material and community participation is covered by the PHIResearchLab.org [Disclaimer](http://www.phiresearchlab.org/index.php?option=com_content&view=article&id=26&Itemid=15) and [Code of Conduct](http://www.phiresearchlab.org/index.php?option=com_content&view=article&id=27&Itemid=19). For more information about CDC's privacy policy, please visit <http://www.cdc.gov/privacy.html>.

## Contributing Standard Notice

Anyone is encouraged to contribute to the repository by [forking](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) and submitting a pull request. (If you are new to GitHub, you might start with a [basic tutorial](https://help.github.com/en/github/getting-started-with-github/set-up-git).) By contributing to this project, you grant a world-wide, royalty-free, perpetual, irrevocable, non-exclusive, transferable license to all users under the terms of the [Apache Software License v2](http://www.apache.org/licenses/LICENSE-2.0.html) or later.

All comments, messages, pull requests, and other submissions received through CDC including this GitHub page may be subject to applicable federal law, including but not limited to the Federal Records Act and may be archived. Learn more at <http://www.cdc.gov/other/privacy.html>.

## Records Management Standard Notice

This repository is not a source of government records, but is a copy to increase collaboration and collaborative potential. All government records will be published through the [CDC web site.](http://www.cdc.gov)

## Additional Standard Notices

Please refer to [CDC's Template Repository](https://github.com/CDCgov/template/blob/master/open_practices.md) for more information about [contributing to this repository, public domain notices and disclaimers](https://github.com/CDCgov/template/blob/master/open_practices.md), and [code of conduct.](https://github.com/CDCgov/template/blob/master/code-of-conduct.md)

## Learn more about CDC GitHub Practices for Open Source Projects

<https://github.com/CDCgov/template/blob/master/open_practices.md>

**General disclaimer** This repository was created for use by CDC programs to collaborate on public health related projects in support of the [CDC mission](https://github.com/CDCgov/template/blob/master/open_practices.md). Github is not hosted by the CDC, but is a third party website used by CDC and its partners to share information and collaborate on software. CDC’s use of GitHub does not imply an endorsement of any one particular service, product, or enterprise.
