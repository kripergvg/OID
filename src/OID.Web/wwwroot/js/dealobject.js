String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

$(function () {
    var $checkList = $("#checkList");
    var $bodyCheckList = $checkList.find("tbody");

    var $checkType = $("#checkType");
    var $checkDescription = $("#checkDescription");
    var $checkImage = $("#checkImage");

    var currentBase64Image;

    var reader = new FileReader();

    reader.onloadend = function () {
        currentBase64Image = reader.result;
    }

    $checkImage.change(function () {
        reader.readAsDataURL($checkImage[0].files[0]);
    });

    var currentIndex = $bodyCheckList.find("tr").length;

    var checkTemplate = "<tr>\
                    <td>\
                        <input type=\"hidden\" name=\"ObjectChecks.Index\" value=\"%index\"/>\
                        <input type=\"hidden\" name=\"ObjectChecks[%index].CheckType\" value=\"%checkType\"/>\
                          %checkTypeText\
                    </td>\
                    <td>\
                        <input type=\"hidden\" name=\"ObjectChecks[%index].Description\" value=\"%description\" />\
                        %description\
                    </td>\
                    <td>\
                        <input type=\"hidden\" name=\"ObjectChecks[%index].ImageUrl\" value=\"%imageUrl\" />\
                        <img src=\"%image\"  style=\"max-height: 50px; max-width: 50px;\"/>\
                    </td>\
                    <td>\
                        <button class=\"btn btn-danger checkListDelete\" type=\"button\"><span class=\"glyphicon glyphicon-trash\"></span></button>\
                    </td>\
                </tr>";

    function getCheckLine(checkTypeText, checkType, description, image, index) {
        return checkTemplate
            .replace("%checkType", checkType)
            .replace("%checkTypeText", checkTypeText)
            .replaceAll("%description", description)
            .replaceAll("%image", image)
            .replaceAll("%index", index);
    }

    function clearCheckForm() {
        $checkDescription.val("");
        $checkImage.val("");
        currentBase64Image = null;
    }

    $("#addCheck")
        .click(function () {
            $checkList.show();

            var checkType = $checkType.val();
            var checkTypeText = $checkType.find("option:selected").text();
            var checkDescription = $checkDescription.val();                     

            var checkLine = getCheckLine(checkTypeText, checkType, checkDescription, currentBase64Image, currentIndex);
            $bodyCheckList.append(checkLine);
            currentIndex++;
            clearCheckForm();
        });

    $checkList.on("click", ".checkListDelete", function () {
        $(this).parents("tr").remove();
    });
});