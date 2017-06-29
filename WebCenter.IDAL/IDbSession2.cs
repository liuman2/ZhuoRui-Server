 

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
   
	  

		IDAL.Iabroad_historyRepository abroad_historyRepository { get; }
	  

		IDAL.Iabroad_shareholderRepository abroad_shareholderRepository { get; }
	  

		IDAL.IaccountingRepository accountingRepository { get; }
	  

		IDAL.Iaccounting_itemRepository accounting_itemRepository { get; }
	  

		IDAL.Iaccounting_progressRepository accounting_progressRepository { get; }
	  

		IDAL.Iannual_examRepository annual_examRepository { get; }
	  

		IDAL.IareaRepository areaRepository { get; }
	  

		IDAL.IattachmentRepository attachmentRepository { get; }
	  

		IDAL.IauditRepository auditRepository { get; }
	  

		IDAL.Iaudit_bankRepository audit_bankRepository { get; }
	  

		IDAL.IbankRepository bankRepository { get; }
	  

		IDAL.Ibank_accountRepository bank_accountRepository { get; }
	  

		IDAL.IcontactRepository contactRepository { get; }
	  

		IDAL.IcustomerRepository customerRepository { get; }
	  

		IDAL.Icustomer_timelineRepository customer_timelineRepository { get; }
	  

		IDAL.IdictionaryRepository dictionaryRepository { get; }
	  

		IDAL.Idictionary_groupRepository dictionary_groupRepository { get; }
	  

		IDAL.IhistoryRepository historyRepository { get; }
	  

		IDAL.Ihistory_shareholderRepository history_shareholderRepository { get; }
	  

		IDAL.IincomeRepository incomeRepository { get; }
	  

		IDAL.Iinternal_historyRepository internal_historyRepository { get; }
	  

		IDAL.Iinternal_shareholderRepository internal_shareholderRepository { get; }
	  

		IDAL.IleaveRepository leaveRepository { get; }
	  

		IDAL.IlectureRepository lectureRepository { get; }
	  

		IDAL.Ilecture_customerRepository lecture_customerRepository { get; }
	  

		IDAL.ImailRepository mailRepository { get; }
	  

		IDAL.ImemberRepository memberRepository { get; }
	  

		IDAL.ImenuRepository menuRepository { get; }
	  

		IDAL.InoticeRepository noticeRepository { get; }
	  

		IDAL.IoperationRepository operationRepository { get; }
	  

		IDAL.IorganizationRepository organizationRepository { get; }
	  

		IDAL.IpatentRepository patentRepository { get; }
	  

		IDAL.IpositionRepository positionRepository { get; }
	  

		IDAL.IreceiptRepository receiptRepository { get; }
	  

		IDAL.Ireg_abroadRepository reg_abroadRepository { get; }
	  

		IDAL.Ireg_historyRepository reg_historyRepository { get; }
	  

		IDAL.Ireg_internalRepository reg_internalRepository { get; }
	  

		IDAL.Ireg_internal_historyRepository reg_internal_historyRepository { get; }
	  

		IDAL.Ireg_internal_itemsRepository reg_internal_itemsRepository { get; }
	  

		IDAL.IroleRepository roleRepository { get; }
	  

		IDAL.Irole_memberRepository role_memberRepository { get; }
	  

		IDAL.Irole_memuRepository role_memuRepository { get; }
	  

		IDAL.Irole_operationRepository role_operationRepository { get; }
	  

		IDAL.IsequenceRepository sequenceRepository { get; }
	  

		IDAL.IsettingRepository settingRepository { get; }
	  

		IDAL.Isub_auditRepository sub_auditRepository { get; }
	  

		IDAL.ItimelineRepository timelineRepository { get; }
	  

		IDAL.ItrademarkRepository trademarkRepository { get; }
	  

		IDAL.IwaitdealRepository waitdealRepository { get; }
	}
}