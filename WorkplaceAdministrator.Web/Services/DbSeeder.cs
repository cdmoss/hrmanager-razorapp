using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WorkplaceAdministrator.Web.Data
{
    public static class DbSeeder
    {
        public static bool SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (roleManager.FindByNameAsync("Admin").Result == null)
            {
                IdentityRole<int> admin = new IdentityRole<int>("Admin")
                {
                    NormalizedName = "ADMIN"
                };

                IdentityResult adminResult = roleManager.CreateAsync(admin).Result;

                if (!adminResult.Succeeded || !adminResult.Succeeded)
                {
                    return false;
                }
            }
            if (roleManager.FindByNameAsync("Staff").Result == null)
            {
                IdentityRole<int> staff = new IdentityRole<int>("Staff")
                {
                    NormalizedName = "STAFF"
                };

                IdentityResult staffResult = roleManager.CreateAsync(staff).Result;

                if (!staffResult.Succeeded || !staffResult.Succeeded)
                {
                    return false;
                }
            }
            if (roleManager.FindByNameAsync("Volunteer").Result == null)
            {
                IdentityRole<int> volunteer = new IdentityRole<int>("Volunteer")
                {
                    NormalizedName = "VOLUNTEER"
                };

                IdentityResult volunteerResult = roleManager.CreateAsync(volunteer).Result;

                if (volunteerResult.Succeeded && volunteerResult.Succeeded)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public static void SeedAdmin(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("fbadmin").Result == null)
            {
                AppUser admin = new AppUser();
                admin.UserName = "fbadmin";
                admin.NormalizedUserName = admin.UserName.ToUpper();
                admin.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(admin, "P@$$W0rd").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }
        }

        public static void SeedStaff(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("staff").Result == null)
            {
                AppUser staff = new AppUser();
                staff.UserName = "staff";
                staff.NormalizedUserName = staff.UserName.ToUpper();
                staff.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(staff, "P@$$W0rd").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(staff, "Staff").Wait();
                }
            }
        }

        public static void SeedPositions(FoodBankContext context)
        {
            if (context.Positions.Count() == 0)
            {
                Position all = new Position() { Name = "All" };
                Position warehouse = new Position() { Name = "Warehouse" };
                Position frontstock = new Position() { Name = "Front Stock" };
                Position janitorial = new Position() { Name = "Janitorial" };
                Position generalmaintenance = new Position() { Name = "General Maintenance" };
                Position specialevents = new Position() { Name = "Special Events" };
                Position communityrelations = new Position() { Name = "Community Relations" };

                context.Add(all);
                context.Add(frontstock);
                context.Add(janitorial);
                context.Add(generalmaintenance);
                context.Add(specialevents);
                context.Add(communityrelations);

                context.SaveChanges();
            }
        }

        public static void SeedTestVolunteer(UserManager<AppUser> userManager, FoodBankContext context)
        {
            if (userManager.FindByNameAsync("testvol").Result == null)
            {
                AppUser testVol = new AppUser();
                testVol.UserName = "testvol";
                testVol.NormalizedUserName = testVol.UserName.ToUpper();
                testVol.EmailConfirmed = true;
                testVol.Email = "cdmossing@gmail.com";

                VolunteerProfile vi = new VolunteerProfile()
                {
                    FirstName = "testfirst",
                    LastName = "testlast",
                    Address = "testAddress",
                    City = "testcity",
                    PostalCode = "testpostal",
                    MainPhone = "5555555555",
                    AlternatePhone1 = "5555555555",
                    AlternatePhone2 = "5555555555",
                    Birthdate = DateTime.Now,
                    EmergencyFullName = "testemergency",
                    EmergencyPhone1 = "5555555555",
                    EmergencyPhone2 = "5555555555",
                    EmergencyRelationship = "testrelationship",
                    FoodSafe = true,
                    FirstAid = true,
                    Cpr = true,
                    OtherCertificates = "TestOther",
                    EducationTraining = "testeducation",
                    SkillsInterestsHobbies = "testskills",
                    VolunteerExperience = "testexperience",
                    OtherBoards = "otherboards",
                };

                Reference reference = new Reference()
                {
                    Name = "testref",
                    Volunteer = vi,
                    Phone = "5555555555",
                    Relationship = "testrelation",
                    Occupation = "testoccupataion"
                };

                WorkExperience workExp = new WorkExperience()
                {
                    EmployerName = "testemployer",
                    EmployerAddress = "testaddress",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(20),
                    EmployerPhone = "5555555555",
                    ContactPerson = "testcontact",
                    PositionWorked = "testposition"
                };

                List<Reference> references = new List<Reference>();
                references.Add(reference);
                List<WorkExperience> workExperiences = new List<WorkExperience>();
                workExperiences.Add(workExp);

                vi.References = references;
                vi.WorkExperiences = workExperiences;
                testVol.VolunteerProfile = vi;

                IdentityResult result = userManager.CreateAsync(testVol, "P@$$W0rd").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(testVol, "Volunteer").Wait();
                }
            }
        }
    }
}
