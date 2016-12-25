$(function () {
    var $dealObjectList = $("#dealObjectList");
    var $freeDealObjects = $("#freeDealObjects");
    var $addDealObjectBtn = $("#addDealObject");

    function addToFreeDealObjects($trWithDealObject) {
        var value = $trWithDealObject.find("input.objectId").val();
        var name = $trWithDealObject.find("input.objectName").val();
        $freeDealObjects.append("<option value=\"" + value + "\">" + name + "</option>");
    }

    $dealObjectList.on("click",
        ".deleteDealObject",
        function () {
            var $tr = $(this).parents("tr");
            addToFreeDealObjects($tr);
            $tr.remove();

            if ($freeDealObjects.find("option").length !== 0) {
                $freeDealObjects.prop("disabled", false);
                $addDealObjectBtn.prop("disabled", false);
            }
        });

    var currentIndex = $dealObjectList.find("tr").length;

    var dealObjectTemplate = "   <tr>\
                        <td>\
                            <input type=\"hidden\" name=\"SelectedDealObjects.Index\" value=\"@index\"/>\
                            <input type=\"hidden\" name=\"SelectedDealObjects[@index].ObjectId\" class=\"objectId\"  value=\"@dealObject.ObjectId\"/>\
                            <input type=\"hidden\" name=\"SelectedDealObjects[@index].Name\" class=\"objectName\"  value=\"@dealObject.Name\" />\
                            @dealObject.Name\
                        </td>\
                        <td style=\"width: 95px;\">\
                            <button class=\"btn btn-danger deleteDealObject\" type=\"button\">Удалить</button>\
                        </td>\
                    </tr>";

    function getDealObjectContent(index, objectId, name) {
        return dealObjectTemplate
            .replaceAll("@index", index)
            .replace("@dealObject.ObjectId", objectId)
            .replaceAll("@dealObject.Name", name);
    }

    function getSelectedDealObject() {
        var selectedDealObject = $freeDealObjects.find("option:selected");
        var objectId = selectedDealObject.attr("value");
        var name = selectedDealObject.text();

        return { objectId: objectId, name: name };
    }

    function removeSelectedDealObject() {
        var selectedDealObject = $freeDealObjects.find("option:selected");
        selectedDealObject.remove();
    }

    var $bodyDealObjectList = $dealObjectList.find("tbody");
    $addDealObjectBtn
        .click(function () {

            var dealObject = getSelectedDealObject();
            removeSelectedDealObject();

            var dealObjectHtml = getDealObjectContent(currentIndex, dealObject.objectId, dealObject.name);
            $bodyDealObjectList.append(dealObjectHtml);

            currentIndex++;

            if ($freeDealObjects.find("option").length === 0) {
                $freeDealObjects.prop("disabled", true);
                $addDealObjectBtn.prop("disabled", true);
            }
        });
});

