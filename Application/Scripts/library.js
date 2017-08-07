function updatePage(updateIntervalInMilliseconds, cycles) {
    try {
        if (cycles++ < 10000) {
            PageMethods.UpdateFiles(updateFilesCallback);
            PageMethods.UpdateHyperLinks(updateHyperLinksCallback);
            PageMethods.UpdateImages(updateImagesCallback);
            setTimeout("updatePage(" + updateIntervalInMilliseconds + "," + cycles + ")", updateIntervalInMilliseconds);
        }
    }
    catch (exception) {
        document.writeln("updatePage exception: " + exception);
    }
}

function updateFilesCallback(result) {

    if(result.length == 0){
        return;
    }

    var fileDiscoveryPaths = result.toString().split(",");
    
    for (i = 0; i < fileDiscoveryPaths.length; i++) {
        var fileDiscoveryPath = fileDiscoveryPaths[i].split("|");

        var fileA = new Element('a', { 'class': 'file', id: fileDiscoveryPath[0], 'z-index': 'auto' });
        fileA.href = fileDiscoveryPath[1];
        fileA.innerText = "File: " + fileDiscoveryPath[1] + " ";

        $('canvas').appendChild(fileA);
    }
}

function updateHyperLinksCallback(result) {

    if(result.length == 0){
        return;
    }

    var hyperLinkDiscoveryPaths = result.toString().split(",");
    
    for (i = 0; i < hyperLinkDiscoveryPaths.length; i++) {
        var hyperLinkDiscoveryPath = hyperLinkDiscoveryPaths[i].split("|");

        var hyperLinkA = new Element('a', { 'class': 'hyperLink', id: hyperLinkDiscoveryPath[0], 'z-index': 'auto' });
        hyperLinkA.href = hyperLinkDiscoveryPath[1];
        hyperLinkA.innerText = "HyperLink: " + hyperLinkDiscoveryPath[1] + " ";

        $('canvas').appendChild(hyperLinkA);
    }
}

function updateImagesCallback(result) {

    if(result.length == 0){
        return;
    }

    var imageDiscoveryPaths = result.toString().split(",");
    
    for (i = 0; i < imageDiscoveryPaths.length; i++) {
        var imageDiscoveryPath = imageDiscoveryPaths[i].split("|");

        var imageSpan = new Element('span', { 'class': 'imageSpan', id: imageDiscoveryPath[0], 'z-index': 'auto' });
        var image = new Element('img', { 'class': 'image', src: imageDiscoveryPath[1], opacity: '100', 'z-index': 'auto' });

        $('canvas').appendChild(imageSpan);
        
        imageSpan.hide();

        $(imageSpan).appendChild(image);

        $(imageDiscoveryPath[0]).show();
        $(imageDiscoveryPath[0]).fade({ duration: 1.5, from: 0, to: 1 });

        new Draggable(imageDiscoveryPath[0], { snap: [25, 25] });
    }
}

