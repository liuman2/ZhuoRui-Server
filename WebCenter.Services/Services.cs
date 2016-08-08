 

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
	
	public partial class bank_accountService:BaseService<bank_account>,Ibank_accountService
    {   
        public bank_accountService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.bank_accountRepository;
        }  
    }
	
	public partial class customerService:BaseService<customer>,IcustomerService
    {   
        public customerService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.customerRepository;
        }  
    }
	
	public partial class customer_timelineService:BaseService<customer_timeline>,Icustomer_timelineService
    {   
        public customer_timelineService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.customer_timelineRepository;
        }  
    }
	
	public partial class dictionaryService:BaseService<dictionary>,IdictionaryService
    {   
        public dictionaryService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.dictionaryRepository;
        }  
    }
	
	public partial class dictionary_groupService:BaseService<dictionary_group>,Idictionary_groupService
    {   
        public dictionary_groupService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.dictionary_groupRepository;
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