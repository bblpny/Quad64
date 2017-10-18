# Quad64 v0.1.1
An open-source SM64 level editor written in C# 4.0, and uses Windows Forms and OpenTK.

<a href="http://i.imgur.com/JjbpW9V.png"><img src="http://i.imgur.com/JjbpW9V.png"/></a>

## Features

The main focus of the tool is to be like Toads Tool 64, but with better ROM compatibility.
* Both extended ROMs and vanilla (8MB) ROMs that have uncompressed data can be modified.
* You can modify any of the 4 major regional versions of SM64, which includes: US, Europe, Japan, and Japan (Shindou edition)
* You can load and save ROM files as big endian(.z64), middle endian(.v64), or little endian(.n64)
* Supports most of the N64 texture formats: RGBA16, RGBA32, IA16, IA8, IA4, I8, and I4. (CI textures will be interpreted as grayscale)

## Features in this fork..

* More sensitive to GC allocations, optimized for speed.
* Export everything to wavefront OBJ/MTL (with vertex color for software that supports it, [blender])
* Partial fixes to rotation issues
* Shadows and decals on surfaces render correctly
* More accurate texture colors
* Address format accompanies items within MIO0
* Rendering redone, z sorted alpha blending. (WIP)
* Partial billboard support
* Fixes..

You can modify MIO0 now, but if the recompressed data doesn't fit you wont be able to save certain changes. You should probably still expand the rom.


# Required Libraries

## OpenTK
Required for the 3D Viewer window.

https://www.nuget.org/packages/OpenTK/
<br/>
https://www.nuget.org/packages/OpenTK.GLControl/

## JSON.NET
Required for reading/writing JSON files.

http://www.newtonsoft.com/json

## BubblePony.*
Libraries are included in binary format, source can be found here:

https://github.com/bblpny/csharputilities


# License (MIT)

Copyright (c) 2017 David Benepe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
