$(document).ready(function () {
    flickerAdmin.modeChanged(document.getElementById('Mode'));
    $('#Mode').bind('change keyup', function () { modeChanged(this) });
});

(function (flickerAdmin, $, undefined) { 
    flickerAdmin.modeChanged = function(ddlMode) {
        flickerAdmin.hideAllModes();
        var selectedMode = ddlMode.options[ddlMode.selectedIndex].value;
        $('#' + selectedMode).show();
    }

    flickerAdmin.hideAllModes = function() {
        $(".modeSection").hide();
    }
} (window.flickerAdmin = window.flickerAdmin || {}, jQuery));