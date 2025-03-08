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

# Repository root
ROOTDIR=$( cd -- "$( dirname -- "$0" )/.." &> /dev/null && pwd )

# Check for dependencies
msbuildpath=`which docfx`
if [ ! $? == 0 ]; then
	echo DocFX is not found.
	exit 1
fi

# Turn off telemetry and logo
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# Build KS
echo Building documentation...
docfx $ROOTDIR/DocGen/docfx.json
if [ ! $? == 0 ]; then
	echo Build failed.
	exit 1
fi

# Inform success
echo Build successful.
exit 0
