 

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
   
	
	public partial class annual_examService:BaseService<annual_exam>,Iannual_examService
    {   
        public annual_examService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.annual_examRepository;
        }  
    }
	
	public partial class areaService:BaseService<area>,IareaService
    {   
        public areaService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.areaRepository;
        }  
    }
	
	public partial class auditService:BaseService<audit>,IauditService
    {   
        public auditService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.auditRepository;
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
	
	public partial class incomeService:BaseService<income>,IincomeService
    {   
        public incomeService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.incomeRepository;
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
	
	public partial class menuService:BaseService<menu>,ImenuService
    {   
        public menuService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.menuRepository;
        }  
    }
	
	public partial class operationService:BaseService<operation>,IoperationService
    {   
        public operationService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.operationRepository;
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
	
	public partial class patentService:BaseService<patent>,IpatentService
    {   
        public patentService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.patentRepository;
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
	
	public partial class reg_abroadService:BaseService<reg_abroad>,Ireg_abroadService
    {   
        public reg_abroadService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.reg_abroadRepository;
        }  
    }
	
	public partial class reg_historyService:BaseService<reg_history>,Ireg_historyService
    {   
        public reg_historyService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.reg_historyRepository;
        }  
    }
	
	public partial class reg_internalService:BaseService<reg_internal>,Ireg_internalService
    {   
        public reg_internalService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.reg_internalRepository;
        }  
    }
	
	public partial class reg_internal_historyService:BaseService<reg_internal_history>,Ireg_internal_historyService
    {   
        public reg_internal_historyService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.reg_internal_historyRepository;
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
	
	public partial class role_memberService:BaseService<role_member>,Irole_memberService
    {   
        public role_memberService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.role_memberRepository;
        }  
    }
	
	public partial class role_memuService:BaseService<role_memu>,Irole_memuService
    {   
        public role_memuService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.role_memuRepository;
        }  
    }
	
	public partial class role_operationService:BaseService<role_operation>,Irole_operationService
    {   
        public role_operationService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.role_operationRepository;
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
	
	public partial class settingService:BaseService<setting>,IsettingService
    {   
        public settingService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.settingRepository;
        }  
    }
	
	public partial class timelineService:BaseService<timeline>,ItimelineService
    {   
        public timelineService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.timelineRepository;
        }  
    }
	
	public partial class trademarkService:BaseService<trademark>,ItrademarkService
    {   
        public trademarkService()
        {}
        public override void SetCurrentRepository()
        {
            CurrentRepository = _dbSession.trademarkRepository;
        }  
    }
	
}