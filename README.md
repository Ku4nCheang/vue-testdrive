# Getting Started

## Development

Before running this project, you have to restore the project
by following steps were shown below. Assume you are in the **project root**.

Using build script to start running

```bash
# only call when the first run or packages that are needed to update
npm install
# call the script to run
./build.sh run
```

Using dotnet cli

```bash
# only call when the first run or packages that are needed to update
npm install
# using dotnet watch run can keep restart server when file has changed
cd Server && dotnet run
```

### Updated Vendor.js

when you add a package into `webpack.config.vendor.js` you have to run the webpack to package the new `vendor.js`.

```bash
# call the script to run
./build.sh pack
```

```bash
# generator vendor js using node command
node node_modules/webpack/bin/webpack.js --config webpack.config.vendor.js
```

## Publish Project

Using the build script to help to publish project. (**Recommended**).
It will organize the published folder in well-defined file structure. You just need to copy the zip file in Current folder into the environment that needs to be deployed.

* Releases
  * **Current** - Contains latest released files that have been zipped in `.zip` format and  appsettings.json for each environments `Staging`, `Production` and `Development`. default environment: `Production`
  * Version folders - List of application versions
    * Runtime folders - Runtime platform `win81-x64` (default) and `osx-x64`
      * Published folders - The published folder with timestamp

Publish application with version `v1.0`

```bash
# publish the project under v1.0 folder
./build.sh publish -v v1.0
```

Publish application with osx runtime

```bash
# publish the project under v1.0 folder and osx-x64 runtime
./build.sh publish -v v1.0 -r osx-x64
```

Publish application with different environment. It will embed the corresponding appsettings.json and cert file into the published folders.

```bash
# publish the project under v1.0 folder for QA Environment
./build.sh publish -v v1.0 -e Staging
```