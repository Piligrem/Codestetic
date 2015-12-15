var xmlhttp;

if (window.XMLHttpRequest) {
    // code for IE7+, Firefox, Chrome, Opera, Safari
    xmlhttp = new XMLHttpRequest();
} else {
    // code for IE6, IE5
    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
}

xmlhttp.onreadystatechange = function () {
    if (xmlhttp.readyState == XMLHttpRequest.DONE) {
        if (xmlhttp.status == 200) {
            var result = JSON.parse(xmlhttp.response);
            if (result.success) {
                String.Resources = result.data;
            }
        }
        else {
            $.log("Error: {0}, Message: {1}".format(xmlhttp.status, xmlhttp.responseText), 'Request language resource');
        }
    }
}

xmlhttp.open("POST", "/Common/GetResourceLanguage", true);
xmlhttp.send();