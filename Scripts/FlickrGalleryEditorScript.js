$(document).ready(function () {
    flickrAdmin.cacheOptionChanged(document.getElementById('DisableCaching'));
    flickrAdmin.modeChanged(document.getElementById('Mode'));
    $('#Mode').bind('change keyup', function () { flickrAdmin.modeChanged(this) });
    $('#DisableCaching').bind('change', function () { flickrAdmin.cacheOptionChanged(this) });
});

(function (flickrAdmin, $, undefined) {
    flickrAdmin.modeChanged = function (ddlMode) {
        flickrAdmin.hideAllModes();
        var selectedMode = ddlMode.options[ddlMode.selectedIndex].value;
        $('#' + selectedMode).show();
    }

    flickrAdmin.hideAllModes = function () {
        $(".modeSection").hide();
    }

    flickrAdmin.cacheOptionChanged = function (chkDisableCache) {
        if(chkDisableCache.checked)
            $(".cacheDurationSection").hide();
        else
            $(".cacheDurationSection").show();
    }
} (window.flickrAdmin = window.flickrAdmin || {}, jQuery));