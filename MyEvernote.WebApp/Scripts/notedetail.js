$(function () {

    // yorumları listeleyen fonksiyon
    $('#modal_notedetail').on('show.bs.modal', function (e) {  // sayfa show old. anda , function daki kodlar çalışır

        var btn = $(e.relatedTarget); // tıklanan butonu yakalama
        noteid = btn.data("note-id"); // tıklanan butondaki data-note-id attributenin note-id si kesilerek alınır.

        $("#modal_notedetail_body").load("/Note/GetNoteDetail/" + noteid); // body alanı controller deki action u yükleme
    });
});