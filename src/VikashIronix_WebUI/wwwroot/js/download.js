window.saveAsFile = (filename, bytesBase64) => {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.vikashPrint = (html) => {
    var printWin = window.open('', '_blank', 'width=900,height=700');
    printWin.document.open();
    printWin.document.write(html);
    printWin.document.close();
    // Wait for content to load then print
    printWin.onload = function () {
        printWin.focus();
        printWin.print();
    };
    // Fallback for browsers that fire onload before write completes
    setTimeout(() => {
        try { printWin.focus(); printWin.print(); } catch(e) {}
    }, 600);
};
