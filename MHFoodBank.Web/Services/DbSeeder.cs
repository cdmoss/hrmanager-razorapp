using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting;

namespace MHFoodBank.Web.Data
{
    public static class DbSeeder
    {
        static RoleManager<IdentityRole<int>> roleManager;
        static UserManager<AppUser> userManager;
        public static bool Seed(RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager, FoodBankContext foodBankContext, IWebHostEnvironment env)
        {
            DbSeeder.roleManager = roleManager;
            DbSeeder.userManager = userManager;

            bool result = true;

            result &= SeedRoles();

            if (env.IsDevelopment())
            {
                result &= SeedTestVolunteer();
                result &= SeedStaff();
            }

            result &= SeedAdmin();
            result &= SeedPositions(foodBankContext);

            return result;
        }

        private static bool RoleExists(string roleName)
        {
            return roleManager.FindByNameAsync(roleName).Result != null;
        }

        private static bool UserExists(string userName)
        {
            return userManager.FindByNameAsync(userName).Result != null;
        }

        private static IdentityResult CreateRole(string roleName)
        {
            var role = new IdentityRole<int>(roleName)
            {
                NormalizedName = roleName.ToUpper()
            };

            return roleManager.CreateAsync(role).Result;
        }

        private static bool CreateUser(string userName, string userRole)
        {
            var user = new AppUser()
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                EmailConfirmed = true
            };

            IdentityResult result = userManager.CreateAsync(user, "P@$$W0rd").Result;

            if (result.Succeeded)
                SetUserToRole(user, userRole);
            else
                return false;

            return true;
        }

        private static bool CreateVolunteer(string userName, string userRole)
        {
            var volunteer = new AppUser()
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                EmailConfirmed = true,
                Email = "cdmossing@gmail.com"
            };

            VolunteerProfile vi = new VolunteerProfile()
            {
                FirstName = "testfirst",
                LastName = "testlast",
                Address = "testAddress",
                City = "testcity",
                PostalCode = "T1R1L9",
                MainPhone = "5555555555",
                AlternatePhone1 = "5555555555",
                AlternatePhone2 = "5555555555",
                ApprovalStatus = ApprovalStatus.Pending,
                Birthdate = DateTime.Now,
                EmergencyFullName = "testemergency",
                EmergencyPhone1 = "5555555555",
                EmergencyPhone2 = "5555555555",
                EmergencyRelationship = "testrelationship",
                FoodSafe = false,
                FirstAidCpr = false,
                OtherCertificates = "TestOther",
                EducationTraining = "testeducation",
                SkillsInterestsHobbies = "testskills",
                VolunteerExperience = "testexperience",
                OtherBoards = "otherboards",
            };

            Reference reference = new Reference()
            {
                Name = "Steve",
                Volunteer = vi,
                Phone = "4034056785",
                Relationship = "Instructor",
                Occupation = "Professor"
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
            volunteer.VolunteerProfile = vi;

            IdentityResult result = userManager.CreateAsync(volunteer, "P@$$W0rd").Result;

            if (result.Succeeded)
                SetUserToRole(volunteer, userRole);
            else
                return false;

            return true;
        }

        private static void SetUserToRole(AppUser user, string userRole)
        {
            userManager.AddToRoleAsync(user, userRole).Wait();
        }

        private static bool SeedAdmin()
        {
            if (!UserExists(Constants.UserNames.Admin))
            {
                bool userCreatedAndRoleWasSet = CreateUser(Constants.UserNames.Admin, Constants.RoleNames.Admin);

                if (!userCreatedAndRoleWasSet)
                    return false;
            }
            return true;
        }

        private static bool SeedStaff()
        {
            if (!UserExists(Constants.UserNames.Staff))
            {
                bool userCreatedAndRoleWasSet = CreateUser(Constants.UserNames.Staff, Constants.RoleNames.Staff);

                if (!userCreatedAndRoleWasSet)
                    return false;
            }
            return true;
        }

        private static bool SeedTestVolunteer()
        {
            if (!UserExists(Constants.UserNames.Volunteer))
            {
                bool volunteerCreated = CreateVolunteer(Constants.UserNames.Volunteer, Constants.RoleNames.Volunteer);

                if (!volunteerCreated)
                    return false;
            }
            return true;
        }

        private static bool SeedRoles()
        {
            if (!RoleExists(Constants.RoleNames.Admin))
            {
                IdentityResult adminResult = CreateRole(Constants.RoleNames.Admin);

                if (!adminResult.Succeeded)
                {
                    return false;
                }
            }
            if (!RoleExists(Constants.RoleNames.Staff))
            {
                IdentityResult staffResult = CreateRole(Constants.RoleNames.Staff);

                if (!staffResult.Succeeded)
                {
                    return false;
                }
            }
            if (!RoleExists(Constants.RoleNames.Volunteer))
            {
                IdentityResult volunteerResult = CreateRole(Constants.RoleNames.Volunteer);

                if (!volunteerResult.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool SeedPositions(FoodBankContext context)
        {
            if (context.Positions.Count() == 0)
            {
                try
                {
                    Position all = new Position() { Name = "All" };
                    Position warehouse = new Position() { Name = "Warehouse", Color = "#009933" };
                    Position frontstock = new Position() { Name = "Front Stock", Color = "#0066ff" };
                    Position janitorial = new Position() { Name = "Janitorial", Color = "#009999" };
                    Position generalmaintenance = new Position() { Name = "General Maintenance" };
                    Position specialevents = new Position() { Name = "Special Events", Color = "#cc0099" };
                    Position communityrelations = new Position() { Name = "Community Relations", Color = "#ff6600" };

                    context.Add(all);
                    context.Add(warehouse);
                    context.Add(frontstock);
                    context.Add(janitorial);
                    context.Add(generalmaintenance);
                    context.Add(specialevents);
                    context.Add(communityrelations);

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }

    }
}
