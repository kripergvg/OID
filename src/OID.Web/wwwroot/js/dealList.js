$(function() {
    var $dealListTrs = $("#dealList tbody tr");
    $dealListTrs.click(function() {
        var dealId=$(this).attr("data-deal-id");
        window.location = "/Deal/ViewDeal?dealId=" + dealId;
    });
});