$(function () { // sayfa tamamen yüklndikten sonra çalışan fonksiyon

    var noteids = [];

    // sayfa da yüklenen tüm not id leri diziye atan fonksiyon
    // i=index , e=element
    $("div[data-note-id]").each(function (i, e) {  // sayfada div elementi olup data-note-id attribute ne sahip olan tüm div leri alıp each ile hepsinde dönme
        noteids.push($(e).data("note-id")); //dönen id leri push ile dizi ye atma
    });

    //Sayfa yüklediği anda post işlemi yapılır. dönen id leri ajax ile controller e gönderme ve kullanıcının liked yaptığı ve tüm notları listeleme
    $.ajax({

        method: "POST",
        url: "/Note/GetLiked",
        data: { ids: noteids }

    }).done(function (data) {

        // controller de dönen likedNoteIds değerleri result a atanarak bu fonksiyoonda gelen değerler kullanılır
        if (data.result != null && data.result.length > 0) {

            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                var likednote = $("div[data-note-id=" + id + "]") // div olup data-no-id si gelen notid ye eşit olan notu yani div i bulma
                var btn = likednote.find("button[data-liked]"); // button olup data-liked attribute ne sahip butonu bulma
                var spanStar = btn.find("span.like-star"); // span olupta classı like-star olan spanı bulma

                btn.data("liked", true);
                spanStar.removeClass("glyphicon-heart-empty");
                spanStar.addClass("glyphicon-heart");
            }
        }

        else {
            //alert("not bulunamadı.")
        }

    }).fail(function () {
        //alert("sunucu ile bağlantı kurulamadı.")
    });

    // Notu beğenme ajax metodu
    $("button[data-liked]").click(function () { // data-liked attribıte ne sahip butona tıklama . İlk değer = false 

        var btn = $(this); // this ile o anki butonu yakalama
        var liked = $(btn).data("liked");
        var noteid = $(btn).data("note-id");
        var spanStar = btn.find("span.like-star"); // span olupta classı like-star olan spanı bulma
        var spanCount = btn.find("span.like-count"); // span olupta classı like-count olan spanı bulma

        console.log(liked);
        console.log("like count : " + spanCount.text());

        // controllere ajax isteği gönderme
        $.ajax({

            method: "POST",
            url: "/Note/SetLikeState",
            data: { "noteid": noteid, "liked": !liked }
        }).done(function (data) { // bana controller de gelen sonucu kullan. İşlem başarılı ise

            console.log(data);

            if (data.hasError) // if(data.HasError == true) hata varsa
            {
                alert(data.errorMessage);
            }
            else {
                // like lanma işlemi başarılı ise yapılacak işlmler
                liked = !liked; // like ise tıklanarak unlike yapma yada unlike ise tıklanarak like yapma durumu
                btn.data("liked", liked); // ilk değer false old. buttonun attribute tıklandığında güncelleme yapılır
                spanCount.text(data.result);

                console.log("after like count : " + spanCount.text());

                spanStar.removeClass("glyphicon-heart-empty");
                spanStar.removeClass("glyphicon-heart");

                if (liked) {
                    spanStar.addClass("glyphicon-heart");
                } else {
                    spanStar.addClass("glyphicon-heart-empty");
                }
            }

        }).fail(function () {
            alert("sunucu ile bağlantı kurulamadı.");
        });

    })


})