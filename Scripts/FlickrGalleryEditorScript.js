$(document).ready(function () {
    flickrAdmin.modeChanged(document.getElementById('Mode'));
    $('#Mode').bind('change keyup', function () { flickrAdmin.modeChanged(this) });
});

(function (flickrAdmin, $, undefined) { 
    flickrAdmin.modeChanged = function(ddlMode) {
        flickrAdmin.hideAllModes();
        var selectedMode = ddlMode.options[ddlMode.selectedIndex].value;
        $('#' + selectedMode).show();
    }

    flickrAdmin.hideAllModes = function() {
        $(".modeSection").hide();
    }
} (window.flickrAdmin = window.flickrAdmin || {}, jQuery));