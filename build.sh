usage="$(basename "$0") [pack|run|publish] [-h] [-r runtime] [-e environment] [-v appVersion] -- A script that helps to organize published files.

Command:
    pack     use webpack to pack the vendor.js.
    run      start watch and debug the application.
    publish  compile and publish the project into releases folder.

where:
    -h  show this help text
    -r  set the runtime platform for the application (default: win81-x64)
    -e  set the environment variable for the application. 
        It will automatically embed the corresponding appsettings and certificate files into published folder (default: Production)
    -v  set the version as the folder name of Release subfolder (default: v0.0).
"

if [ -z "$1" -o "$1" = "-h" ]; then
    echo "$usage" >&2
    exit 1
fi

APP="netcore"
# the first argument must be command
# and skip the argument after assigned the value into CMD
CMD=$1; shift
RUNTIME="win81-x64"
ENV="Production"
VER="v0.0"
# remember the current working folder
CPWD="$PWD"
# read the options
while getopts h:r:e:v: option; do
    case "${option}" in
        h)
            echo "$usage" >&2
            exit 1
            ;;
        r) 
            if [ "${OPTARG}" != "win81-x64" -a "${OPTARG}" != "osx-x64" ]; then 
                echo "Unsupported runtime: ${OPTARG}. Current availables: win81-x64, osx-x64."
                exit 1
            else
                 RUNTIME=${OPTARG}
            fi
            ;;
        e) 
            if [ "${OPTARG}" != "Development" -a "${OPTARG}" != "Production" -a "${OPTARG}" != "Staging" ]; then 
                echo "Unsupported environemnt: ${OPTARG}. Current availables: Development, Staging and Production."
                exit 1
            else
                 ENV=${OPTARG}
            fi
            ;;
        v)
            VER=${OPTARG}
            ;;
        *)
            echo "$usage" >&2
            exit 1 
    esac
done

if [ "$CMD" = "run" ]; then
    # run the server in development environment
    cd Server && export ASPNETCORE_ENVIRONMENT=Development && dotnet watch run
elif [ "$CMD" = "publish" ]; then
    clear
    # find the filename for the csproject in server folder
    # good for different project that uses same script
    FNAME=`find ./Server -type f -name '*.csproj'`
    # replace the environment variable into csproject file with specified environment.
    sed -i -E "s/\(<Environment>\)[A-Za-z]*\(<\/Environment>\)/\1${ENV}\2/" "$FNAME"
    # remove the backup file for the csproject.
    rm "${FNAME}-e"
    # get current datetime
    DATE=`date '+%Y%m%d%H%M%S'`
    # release file name
    RNAME="${APP}-${DATE}"
    # full path for this release
    OUPUT="Releases/${VER}/${RUNTIME}/${RNAME}"
    # start publish into specified folder
    cd Server && dotnet publish -c Release -r "$RUNTIME" -o "../${OUPUT}"
    # back project root
    cd "${CPWD}"
    # clean up current release file
    rm -rf "Releases/Current"
    mkdir "Releases/Current"
    # copy each environment settings files into current folder
    for e in Development Staging Production
    do
        cp -r "Server/Resources/${e}" "Releases/Current/" 
    done
    # rollback environment variable in csproject file
    sed -i -E "s/\(<Environment>\)[A-Za-z]*\(<\/Environment>\)/\1Development\2/" "$FNAME"
    rm "${FNAME}-e"
    # go to the ouput folder and zip the published files
    cd $OUPUT
    echo "Start zipping published files in ${OUPUT}"
    zip -r -X "${APP}.zip" *
    # move the zip file into current release folder
    mv "${APP}.zip" "${CPWD}/Releases/Current/"
    echo "Publish files has been zipped: ${CPWD}/Releases/Current/"
elif [ "$CMD" = "pack" ]; then
    node node_modules/webpack/bin/webpack.js --config webpack.config.vendor.js 
fi