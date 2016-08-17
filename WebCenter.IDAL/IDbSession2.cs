 

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebCenter.IDAL
{
    public partial interface IDbSession
    {
   
	  

		IDAL.IareaRepository areaRepository { get; }
	  

		IDAL.IauditRepository auditRepository { get; }
	  

		IDAL.Ibank_accountRepository bank_accountRepository { get; }
	  

		IDAL.IcustomerRepository customerRepository { get; }
	  

		IDAL.Icustomer_timelineRepository customer_timelineRepository { get; }
	  

		IDAL.IdictionaryRepository dictionaryRepository { get; }
	  

		IDAL.Idictionary_groupRepository dictionary_groupRepository { get; }
	  

		IDAL.IincomeRepository incomeRepository { get; }
	  

		IDAL.ImemberRepository memberRepository { get; }
	  

		IDAL.IorganizationRepository organizationRepository { get; }
	  

		IDAL.IpatentRepository patentRepository { get; }
	  

		IDAL.IpositionRepository positionRepository { get; }
	  

		IDAL.Ireg_abroadRepository reg_abroadRepository { get; }
	  

		IDAL.Ireg_historyRepository reg_historyRepository { get; }
	  

		IDAL.Ireg_internalRepository reg_internalRepository { get; }
	  

		IDAL.Ireg_internal_historyRepository reg_internal_historyRepository { get; }
	  

		IDAL.IroleRepository roleRepository { get; }
	  

		IDAL.IsequenceRepository sequenceRepository { get; }
	  

		IDAL.IsettingRepository settingRepository { get; }
	  

		IDAL.ItimelineRepository timelineRepository { get; }
	  

		IDAL.ItrademarkRepository trademarkRepository { get; }
	}
}