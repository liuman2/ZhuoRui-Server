 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCenter.Entities;
using WebCenter.IServices;
 
using WebCenter.IDAL;
using WebCenter.DAL;
namespace WebCenter.Services
{
   
	
	public partial class areaService:BaseService<area>,IareaService
    {   
        public areaService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.areaRepository;
        }  
    }
	
	public partial class memberService:BaseService<member>,ImemberService
    {   
        public memberService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.memberRepository;
        }  
    }
	
	public partial class organizationService:BaseService<organization>,IorganizationService
    {   
        public organizationService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.organizationRepository;
        }  
    }
	
	public partial class positionService:BaseService<position>,IpositionService
    {   
        public positionService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.positionRepository;
        }  
    }
	
	public partial class roleService:BaseService<role>,IroleService
    {   
        public roleService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.roleRepository;
        }  
    }
	
	public partial class sequenceService:BaseService<sequence>,IsequenceService
    {   
        public sequenceService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.sequenceRepository;
        }  
    }
	
}