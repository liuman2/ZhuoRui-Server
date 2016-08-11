﻿ 

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
	  

		IDAL.Ibank_accountRepository bank_accountRepository { get; }
	  

		IDAL.IcustomerRepository customerRepository { get; }
	  

		IDAL.Icustomer_timelineRepository customer_timelineRepository { get; }
	  

		IDAL.IdictionaryRepository dictionaryRepository { get; }
	  

		IDAL.Idictionary_groupRepository dictionary_groupRepository { get; }
	  

		IDAL.IincomeRepository incomeRepository { get; }
	  

		IDAL.ImemberRepository memberRepository { get; }
	  

		IDAL.IorganizationRepository organizationRepository { get; }
	  

		IDAL.IpositionRepository positionRepository { get; }
	  

		IDAL.Ireg_abroadRepository reg_abroadRepository { get; }
	  

		IDAL.IroleRepository roleRepository { get; }
	  

		IDAL.IsequenceRepository sequenceRepository { get; }
	  

		IDAL.ItimelineRepository timelineRepository { get; }
	}
}