#searchIEEE

IEEE Registration Authority assignment search (GUI and command line).

GUI Usage
---------

At initial startup of the GUI version, a local copy of the IEEE's assignment databases will be downloaded to your computer, theses local copies are use for search by both GUI and command line version. The databases are stored as CSV files in the following location

    %USERPROFILE%\AppData\Roaming\Eddy Beaupré\searchIEEE\

To search the databases, simply enter your search term then press ENTER, If any records are found a result window will open and show all matching results. The search term will be match agains the assignment number, organisation name and address. All matching results will be return. MAC address match is not based on any predefined format, you can enter MAC in any format (aabb.ccdd.eeff, aa-bb-cc-dd-ee-ff, aa:bb:cc:dd:ee:ff, aabbccddeeff, etc...)

Pressing ENTER without any search term will show the entiere database.

The configure button can be used to change the URL used to fetch the databases, there should be no reason to change theses unless IEEE decide to move theses files elsewhere. And the refresh button download the lastest version avalable of all databases. Since IEEE add new entries daily, it is a good idead to refresh your local copies at regular interval.

Command line Usage
------------------

    %ProgramFiles(x86)%\Eddy Beaupré\searchIEEE\searchIEEE-Console.exe
    searchIEEE 1.3.0.0
    Copyright ©  2016 Eddy Beaupré
    Usage: searchIEEE-Console.exe [-s term] [-a]
    
      -s, --search    Search term.
    
      -a, --all       Show all entries.
    
      --help          Display this help screen.

Since version 1.3, the command line version is also able to initialize the databases if needed. Results are printed in CSV format:

    <ID>,<ASSIGNMENT>,<DATABASE>,<ORGANISATION NAME>,<ORGANISATION ADDRESS>

Licence
-------
Copyright (c) 2016 Eddy Beaupré. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
