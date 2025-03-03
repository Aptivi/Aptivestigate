#!/bin/bash

#   Aptivestigate  Copyright (C) 2024-2025  Aptivi
#  
#   This file is part of Aptivestigate
#  
#   Aptivestigate is free software: you can redistribute it and/or modify
#   it under the terms of the GNU General Public License as published by
#   the Free Software Foundation, either version 3 of the License, or
#   (at your option) any later version.
#  
#   Aptivestigate is distributed in the hope that it will be useful,
#   but WITHOUT ANY WARRANTY, without even the implied warranty of
#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#   GNU General Public License for more details.
#  
#   You should have received a copy of the GNU General Public License
#   along with this program.  If not, see <https://www.gnu.org/licenses/>.

# Convenience functions
checkerror() {
    if [ $1 != 0 ]
    then
        printf "$2 - Error $1\n" >&2
        exit $1
    fi
}

# This script builds. Use when you have dotnet installed.
releaseconf=$1
if [ -z $releaseconf ]; then
	releaseconf=Release
fi

# Check for dependencies
dotnetpath=`which dotnet`
checkerror $? "dotnet is not found"

# Turn off telemetry and logo
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# Download packages
echo Downloading packages...
"$dotnetpath" restore "../Aptivestigate.sln" -p:Configuration=$releaseconf ${@:2}
checkerror $? "Failed to download packages"

# Build
echo Building...
"$dotnetpath" build "../Aptivestigate.sln" -p:Configuration=$releaseconf ${@:2}
checkerror $? "Failed to build Aptivestigate"

# Inform success
echo Build successful.
