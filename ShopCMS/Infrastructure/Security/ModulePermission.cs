using System;
using System.Collections.Generic;
using System.Linq;
using Domain;


namespace ahmadi.Infrastructure.Security
{
    public static class ModulePermission
    {
        public static bool check(string userId, int moduleId, Int16? typeAccess)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (typeAccess.HasValue)
                {
                    if (uow.AdministratorPermissionRepository.Get(x=>x,x => x.UserId == userId && x.ModuleId == moduleId && x.TypeAccess == typeAccess).Any())
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (uow.AdministratorPermissionRepository.Get(x=>x,x => x.UserId == userId && x.ModuleId == moduleId).Any())
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                uow.Dispose();
            }
        }

        public static List<bool> check(string userId, int moduleId)
        {
            List<bool> permissions = new List<bool>();
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = uow.AdministratorPermissionRepository.Get(x=>x,x => x.UserId == userId && x.ModuleId == moduleId);
                if (p.Where(x => x.TypeAccess == 1).Any())
                    permissions.Add(true);
                else
                    permissions.Add(false);
                if (p.Where(x => x.TypeAccess == 2).Any())
                    permissions.Add(true);
                else
                    permissions.Add(false);
                if (p.Where(x => x.TypeAccess == 3).Any())
                    permissions.Add(true);
                else
                    permissions.Add(false);

                return permissions;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                uow.Dispose();
            }
        }
    }
}
