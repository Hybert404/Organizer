using Organizer.Enums;
using Organizer.Models;
using Organizer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Organizer.Services
{
    public class DbService
    {
        private DataClasses1DataContext _context;

        public DbService()
        {
            _context = new DataClasses1DataContext();
        }

        public List<Time_program> GetProgramTimeListByProgramId(int id, DateTime startDate, DateTime endDate)
        {
            return _context.Time_program.Where(tp => tp.Id_prog == id && tp.Time_start >= startDate && tp.Time_start <= endDate).ToList();
        }

        public List<Time_profile> GetProfileTimeListByProfileId(int id, DateTime startDate, DateTime endDate)
        {
            return _context.Time_profile.Where(tp => tp.Id_prof == id && tp.Time_start >= startDate && tp.Time_start <= endDate).ToList();
        }
        public List<Time_profile> GetAllProfileTimeListByProfileId(int id)
        {
            return _context.Time_profile.Where(tp => tp.Id_prof == id).ToList();
        }

        public Profile GetProfileByName(string name)
        {
            return _context.Profile.Where(p => p.Name == name).FirstOrDefault();
        }

        public List<Profile> GetAllProfiles()
        {
            return _context.Profile.ToList();
        }

        public List<Program> GetProgramListByProfileName(string profileName)
        {
            var profile = _context.Profile.Where(x => x.Name == profileName).FirstOrDefault();
            var programDescList = _context.Program_desc;
            return _context.Program.Where(p => programDescList.Where(pd => pd.Id_prof == profile.Id_prof).Any(pd => pd.Id_prog == p.Id_prog)).ToList();
        }

        public List<TimeInfo> GetProgramTimeInfoById(int id, DateTime StartDate, DateTime EndDate)
        {
            return _context.Time_program.Where(t => t.Id_prog == id && t.Time_start >= StartDate && t.Time_start <= EndDate)
                .Join(_context.Program, t => t.Id_prog, p => p.Id_prog, (t, p) => 
                    new TimeInfo() 
                    {
                        Id = t.Id_prog,
                        Name = p.Name,
                        StartTime = t.Time_start,
                        StopTime = t.Time_stop 
                    }
                ).ToList();
        }

        public List<TimeInfo> GetProfileTimeInfoById(int id, DateTime StartDate, DateTime EndDate)
        {
            return _context.Time_profile.Where(t => t.Id_prof == id && t.Time_start >= StartDate && t.Time_start <= EndDate)
                .Join(_context.Profile, t => t.Id_prof, p => p.Id_prof, (t, p) =>
                    new TimeInfo()
                    {
                        Id = t.Id_prof,
                        Name = p.Name,
                        StartTime = t.Time_start,
                        StopTime = t.Time_stop
                    }
                ).ToList();
        }
        public List<Program_desc> GetProgramDescByProfileId(int id)
        {
            var profile = _context.Profile.Where(x => x.Id_prof == id).FirstOrDefault();
            return _context.Program_desc.Where(x => x.Id_prof == id).ToList();
        }
    }
}
