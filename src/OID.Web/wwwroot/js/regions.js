$(function () {
    function updateLocationsByLocality(locality) {
        $.ajax({
            url: "/region/GetLocationsByLocality?localityCode=" + locality
        })
            .done(function (data) {
                var $locationList = $("#locationList");
                $locationList.empty();

                var selectListItems = createSelectListItems(data);
                $locationList.html(selectListItems);
            });
    }

    function createSelectListItems(items) {
        var selectListItems = "";

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            selectListItems += "<option value='" + item.code + "'>" + item.name + "</option>";
        }

        return selectListItems;
    }

    var $localityList = $("#localityList");
    var $regionList = $("#regionList");

    $regionList
        .change(function () {
            var newRegionCode = $regionList.val();

            $.ajax({
                url: "/region/GetLocalities?regionCode=" + newRegionCode
            })
                .done(function (data) {
                    $localityList.empty();

                    var selectListItems = createSelectListItems(data);
                    $localityList.html(selectListItems);

                    updateLocationsByLocality(data[0].code);
                });

            $.ajax({
                url: "/region/GetLocationsByRegion?regionCode=" + newRegionCode
            })
                .done(function (data) {
                    var $cityList = $("#cityList");
                    $cityList.empty();

                    var selectListItems = createSelectListItems(data);
                    $cityList.html(selectListItems);
                });
        });

    $localityList.change(function() {
        var newLocalityCode = $localityList.val();
        updateLocationsByLocality(newLocalityCode);
    });
});