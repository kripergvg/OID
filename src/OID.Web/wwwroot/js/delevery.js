$(function () {
    //$("input[data-radio-identifier=deleveryType]")
    //    .change(function () {
    //        var regionType = $("input[data-radio-identifier=deleveryType]:checked").val();
    //        var $regions = $("div[data-radio-identifier=deleveryType]");
    //        $regions.hide();
    //        $regions.closest("[data-region='" + regionType + "']").show();
    //    });

    $("input[data-radio-identifier]")
       .change(function () {
           var regionIdentifier = $(this).attr("data-radio-identifier");
           var regionType = $("input[data-radio-identifier=" + regionIdentifier + "]:checked").val();
           var $regions = $("div[data-radio-identifier="+regionIdentifier+"]");
           $regions.hide();
           $regions.closest("[data-region='" + regionType + "']").show();
       });
});