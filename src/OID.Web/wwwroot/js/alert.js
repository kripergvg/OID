function ConfirmAlert(options) {

    this.open = function () {
        $("#confirmAlert").modal();
    }

    $(function() {
        var confirmBtn = $("#btnConfirm");
        confirmBtn.click(function() {
            options.onConfirm();
        });
    });
}

function deleteObject(formId) {
    var onConfirm=function() {
        $(formId).submit();
    }
    var options = { onConfirm: onConfirm };

    var alert = new ConfirmAlert(options);
    alert.open();
}