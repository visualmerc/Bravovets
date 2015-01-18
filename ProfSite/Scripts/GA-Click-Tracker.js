$(document).ready(function () {
    if (typeof (_gat) == 'object') {
        $('a').each(function () {
            var uri = $(this).attr('href');
            if (uri) {
                if (uri.match(/^mailto/i)) {
                    $(this).click(function () {
                        ahTracker._trackEvent('mailto', 'click', document.domain + ' | ' + uri.substr(7)); siteTracker._trackEvent('mailto', 'click', uri.substr(7));
                    });
                } else if (uri.match(/^https?:/i)) {
                    $(this).click(function () {
                        ahTracker._trackEvent('external', 'click', document.domain + ' | ' + uri); siteTracker._trackEvent('external', 'click', uri);
                    });
                } else if (uri.match(/\.[a-z]+$/i) && !uri.match(/\.(aspx?|html?)([\?#].*)?$/i)) {
                    var filetype = uri.substr(uri.length - 3);
                    $(this).click(function () {
                        ahTracker._trackEvent('downloads', filetype.toUpperCase(), document.domain + ' | ' + uri);
                        siteTracker._trackEvent('downloads', filetype.toUpperCase(), uri);
                    });
                }
            }
        });
    }
});