$(function () {
    $("input[data-radio-identifier=deleveryType]")
        .change(function() {
            var regionType = $("input[data-radio-identifier=deleveryType]:checked").val();
            var $regions = $("div[data-radio-identifier=deleveryType]");
            $regions.hide();
            $regions.closest("[data-region='" + regionType + "']").show();
        });
});