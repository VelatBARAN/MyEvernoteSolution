var noteid = -1;
var modalComentBodyId = "#modal_comment_body";

$(function () {

    // yorumları listeleyen fonksiyon
    $('#modal_comment').on('show.bs.modal', function (e) {  // sayfa show old. anda , function daki kodlar çalışır

        var btn = $(e.relatedTarget); // tıklanan butonu yakalama
        noteid = btn.data("note-id"); // tıklanan butondaki data-note-id attributenin note-id si kesilerek alınır.

        $(modalComentBodyId).load("/Comment/ShowNoteComments/" + noteid); // body alanı controller deki action u yükleme
    });
});


// yorumları güncelleme-silme işlmeleri yapan fonksiyon
function doComment(btn, mode_event, commentid, span_comment_text_id) {

    var button = $(btn); // gelen butonu jquery ile ele aldık. yoksa aşağıdaki attribute ları bulamaz.
    var mode = button.data("edit-mode"); // buttonun edit modunu oku

    if (mode_event == "edit_clicked") {

        if (!mode) {
            console.log("editable");
            button.data("edit-mode", true); // data-edit-mode="false" button daki edit-mode değeri true yapılarak editleme penceresi açılır
            button.removeClass("btn-warning");
            button.addClass("btn-success");
            var btnSpan = button.find("span"); // button un altındaki span ı bulma
            btnSpan.removeClass("glyphicon-edit");
            btnSpan.addClass("glyphicon-ok");
            $(button).attr("title", 'Kaydet');
            $(span_comment_text_id).addClass("editable");
            $(span_comment_text_id).attr("contenteditable", true);
            $(span_comment_text_id).focus();
        }
        else {
            button.data("edit-mode", false); // data-edit-mode="false" button daki edit-mode değeri true yapılarak editleme penceresi açılır
            button.addClass("btn-warning");
            button.removeClass("btn-success");
            var btnSpan = button.find("span"); // button un altındaki span ı bulma
            btnSpan.addClass("glyphicon-edit");
            btnSpan.removeClass("glyphicon-ok");
            $(button).attr("title", 'Düzenle')
            $(span_comment_text_id).removeClass("editable");
            $(span_comment_text_id).attr("contenteditable", false);

            var span_text = $(span_comment_text_id).text();

            $.ajax({

                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { Text: span_text }
            }).done(function (data) {   // ajax metodu başarlı bir şekilde gerçekleşirse çalışır

                if (data.result) {
                    // yorumlar tekrar yüklenir
                    $(modalComentBodyId).load("/Comment/ShowNoteComments/" + noteid); // body alanı controller deki action u yükleme
                }
                else {
                    alert("yorum güncellenirken hata oluştu");
                }

            }).fail(function () { // bir hata oluşursa çalışan fonksiyon
                alert("Sunucu ile bağlantı kurulamadı.")
            });
        }

    }
    else if (mode_event == "delete_clicked") {

        var dialog_res = confirm("Yorum Silinsin mi?");
        if (!dialog_res) return false;

        $.ajax({

            method: "GET",
            url: "/Comment/Delete/" + commentid

        }).done(function (data) {

            if (data.result) {
                // yorumlar tekrar yüklenir
                $(modalComentBodyId).load("/Comment/ShowNoteComments/" + noteid); // body alanı controller deki action u yükleme
            }
            else {
                alert("yorum silinirken hata oluştu");
            }

        }).fail(function () { // bir hata oluşursa çalışan fonksiyon
            alert("Sunucu ile bağlantı kurulamadı.")
        });
    }
    else if (mode_event == "new_clicked") {

        var new_comment_text = $("#new_comment_text_id").val();

        $.ajax({

            method: "POST",
            url: "/Comment/Create",
            data: { "text": new_comment_text, "noteid": noteid }  // çift tırnak alarak bunun bir property adı olduğunu gösterdik

        }).done(function (data) {

            if (data.result) {
                // yorumlar tekrar yüklenir
                $(modalComentBodyId).load("/Comment/ShowNoteComments/" + noteid); // body alanı controller deki action u yükleme
            }
            else {
                alert("yorum eklenirken hata oluştu");
            }

        }).fail(function () {
            alert("sunucu ile bağlantı kurulamadı.")
        });
    }

}