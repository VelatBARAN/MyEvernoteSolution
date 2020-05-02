using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {

            // Adding admin user
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Welat",
                Surname = "BARAN",
                Usurname = "welatbaran",
                Email = "baranvelat021@gmail.com",
                ProfileImageFile = "user.png",
                Password = Crypto.Hash("liceli21","MD5"),
                IsActive = true,
                IsAdmin = true,
                ActivateGuid = Guid.NewGuid(),
                CreateOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsurname = "welatbaran"
            };

            // Adding standart user
            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "Remzi",
                Surname = "BARAN",
                Usurname = "remzibaran",
                Email = "remzibaran1@gmail.com",
                ProfileImageFile = "user.png",
                Password = Crypto.Hash("remzi21","MD5"),
                IsActive = true,
                IsAdmin = false,
                ActivateGuid = Guid.NewGuid(),
                CreateOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsurname = "remzibaran"
            };

            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);

            for (int m = 0; m < 8; m++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Usurname = $"user{m}",
                    Email = FakeData.NetworkData.GetEmail(),
                    ProfileImageFile = $"user_{m}.png",
                    Password = Crypto.Hash("123456","MD5"),
                    IsActive = true,
                    IsAdmin = false,
                    ActivateGuid = Guid.NewGuid(),
                    CreateOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsurname = $"user{m}"
                };

                context.EvernoteUsers.Add(user);
            }

            context.SaveChanges();

            // Adding list users
            List<EvernoteUser> userList = context.EvernoteUsers.ToList();
            for (int i = 0; i < 10; i++)
            {
                // Adding Fakedata Categorie
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreateOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsurname = "welatbaran"
                };

                context.Categories.Add(cat);

                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++) // Min = 5 , Max = 9
                {
                    EvernoteUser note_owner = userList[FakeData.NumberData.GetNumber(0, userList.Count - 1)];

                    // Adding Fakedata Notes
                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)), // min = 5 yada max = 25 kelime yazar
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1,3)),// min = 1 yada max = 3 paragraf
                        //Category = cat,  --> aşağıda cat.Notes.Add(note); yazıldığı için yazıladabilir yazılmayadabilir
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1,9),
                        Owner = note_owner,
                        CreateOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1),DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsurname = note_owner.Usurname
                    };

                    cat.Notes.Add(note); // Category class ında consactor u oluşturulup otomatik olarak liste oluştuğunda null dönmesi engellendi

                    // Adding Fakedata Comments

                    for (int j = 0; j < FakeData.NumberData.GetNumber(3,5); j++)
                    {
                        EvernoteUser comment_owner = userList[FakeData.NumberData.GetNumber(0, userList.Count - 1)];
                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            //Note = note, aşağıda note.Comments.Add(comment); yazıldığı için yazıladabilir yazılmayadabilir
                            Owner = comment_owner,
                            CreateOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsurname = comment_owner.Usurname
                        };

                        note.Comments.Add(comment);
                    }

                    // Adding fake likes
                    for (int n = 0; n < note.LikeCount; n++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userList[n],
                            //Note = note // yazıladabilir yazılmayadabilir
                        };

                        note.Likes.Add(liked);
                    }

                }

            }

            context.SaveChanges();
        }
    }
}
