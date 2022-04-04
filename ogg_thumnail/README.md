## OGG Thumbnail
Adds images/thumbnails to ogg/opus files

### Usage

 you can run [add_thumbnail.py](add_thumbnail.py), 
    (show help command, listed below)
    ```
    python add_thumbnail.py -h
    ```

 or

 if you just want to generate the metadata file run [ffmeta.py](ffmeta.py),
    (show help command, listed below)
    ```
    python ffmeta.py -i 'filename.jpg'  -i 'anotherfile.jpg'
    >> filename.jpg.base64
    >> anotherfile.jpg.base64
    ```

### Help

 [add_thumbnail.py](add_thumbnail.py)
 ```
 usage: add_thumbnail.py [-h] [-a FILE] [-j FILE] [-o FILE]

 General Options:
  -h, --help               Print this help message and exit
  -a FILE, --audio FILE
                           Specify the audio file.
  -j FILE, --jpg FILE      Specify the jpg file.
  -o FILE, --output FILE
                           Specify the output audio filename.             
 ```

