call AddColumnUnlessExists(Database(), 'accounting', 'tax', 'tinyint(3) DEFAULT NULL COMMENT "是否含税"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_name', 'varchar(200) DEFAULT NULL COMMENT "开票信息名称"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_tax', 'varchar(200) DEFAULT NULL COMMENT "纳税人识别号"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_address', 'varchar(200) DEFAULT NULL COMMENT "开票信息地址"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_tel', 'varchar(20) DEFAULT NULL COMMENT "开票信息电话"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_bank', 'varchar(100) DEFAULT NULL COMMENT "开票信息开户行"');
call AddColumnUnlessExists(Database(), 'accounting', 'invoice_account', 'varchar(100) DEFAULT NULL COMMENT "开票信息开户行账号"');

DROP TABLE IF EXISTS `accounting_progress`;
CREATE TABLE `accounting_progress` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL,
  `date_start` datetime DEFAULT NULL,
  `progress` varchar(100) DEFAULT NULL COMMENT '进度',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('accounting_progress', '1');

call AddColumnUnlessExists(Database(), 'accounting_progress', 'period', 'varchar(8) DEFAULT NULL COMMENT "账期"');
