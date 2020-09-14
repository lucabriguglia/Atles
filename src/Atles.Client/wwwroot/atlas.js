var atlas = atlas || {};

atlas.interop = {
    addCssFile: function (fileNameToAdd, fileNameToRemove) {
        var hrefToAdd = 'css/' + fileNameToAdd;
        if (!$("link[href='" + hrefToAdd + "']").length) {
            var head = document.getElementsByTagName('HEAD')[0];
            var link = document.createElement('link');
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = hrefToAdd;
            head.appendChild(link);
        }
        var hrefToRemove = 'css/' + fileNameToRemove;
        if ($("link[href='" + hrefToRemove + "']").length) {
            $("link[rel=stylesheet][href='" + hrefToRemove + "']").remove();
        }
    },
    prependScriptToHead: function (script) {
        var head = document.getElementsByTagName('HEAD')[0];
        head.innerHTML = script + head.innerHTML;
    },
    scrollToTarget: function (target) {
        var element = document.getElementById(target);
        element.scrollIntoView();
    },
    changePageTitle: function (title) {
        document.title = title;
    },
    getInputFiles: function (fileInputId) {
        var files = [];
        var fileInput = document.getElementById(fileInputId);
        if (fileInput != null) {
            for (var i = 0; i < fileInput.files.length; i++) {
                files.push(fileInput.files[i].name);
            }
        }
        return files;
    },
    uploadFiles: function (fileInputId) {
        var fileInput = document.getElementById(fileInputId);
        var request = new XMLHttpRequest();
        request.send(new FormData(fileInput));
    },
    acceptCookie: function (cookieString) {
        document.cookie = cookieString;
    }
};