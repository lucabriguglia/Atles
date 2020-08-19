//function addCssFile(fileName) {
//    var head = document.getElementsByTagName('HEAD')[0];
//    var link = document.createElement('link');
//    link.rel = 'stylesheet';
//    link.type = 'text/css';
//    link.href = 'css/' + fileName + '.css';
//    head.appendChild(link); 
//}

window.addCssFile = (fileName) => {
    var head = document.getElementsByTagName('HEAD')[0];
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = 'css/' + fileName + '.css';
    head.appendChild(link); 
};

window.scrollToAnchor = (target) => {
    var element = document.getElementById(target);
    element.scrollIntoView();
};