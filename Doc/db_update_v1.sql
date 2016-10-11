SET FOREIGN_KEY_CHECKS=0;

DROP PROCEDURE
IF EXISTS AddColumnUnlessExists;
delimiter '//'

create procedure AddColumnUnlessExists(
    IN dbName tinytext,
    IN tableName tinytext,
    IN fieldName tinytext,
    IN fieldDef text)
begin
    IF NOT EXISTS (
        SELECT * FROM information_schema.COLUMNS
        WHERE column_name=fieldName
        and table_name=tableName
        and table_schema=dbName
        )
    THEN
        set @ddl=CONCAT('ALTER TABLE ',dbName,'.',tableName,
            ' ADD COLUMN ',fieldName,' ',fieldDef);
        prepare stmt from @ddl;
        execute stmt;
    END IF;
end;
//
delimiter ';'

call AddColumnUnlessExists(Database(), 'reg_abroad', 'is_annual', 'int(20) DEFAULT null');
call AddColumnUnlessExists(Database(), 'reg_internal', 'is_annual', 'int(20) DEFAULT null');
call AddColumnUnlessExists(Database(), 'trademark', 'is_annual', 'int(20) DEFAULT null');
call AddColumnUnlessExists(Database(), 'patent', 'is_annual', 'int(20) DEFAULT null');

call AddColumnUnlessExists(Database(), 'reg_abroad', 'annual_date', 'datetime DEFAULT null COMMENT "上次年检日期"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'annual_date', 'datetime DEFAULT null COMMENT "上次年检日期"');
call AddColumnUnlessExists(Database(), 'trademark', 'annual_date', 'datetime DEFAULT null COMMENT "上次年检日期"');
call AddColumnUnlessExists(Database(), 'patent', 'annual_date', 'datetime DEFAULT null COMMENT "上次年检日期"');


INSERT INTO `settings` VALUES ('2', 'PATENT_PERIOD', '10');
INSERT INTO `settings` VALUES ('3', 'TRADEMARK_PERIOD', '10');

DROP TABLE IF EXISTS `history`;
CREATE TABLE `history` (
  `id` int(11) NOT NULL,
  `source` varchar(30) DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `customer_id` int(11) DEFAULT NULL,
  `order_code` varchar(20) DEFAULT NULL COMMENT '原始单号',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',
  `value` text,
  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',
  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',
  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',
  `progress` varchar(50) DEFAULT NULL COMMENT '注册进度',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  CONSTRAINT `history_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `history_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `history_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `history_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `sequence` VALUES ('history', '0');

-- 联系人
call AddColumnUnlessExists(Database(), 'customer', 'contacts', 'text');

-- 附件
DROP TABLE IF EXISTS `attachment`;
CREATE TABLE `attachment` (
  `id` int(11) NOT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_name` varchar(20) DEFAULT NULL,
  `attachment_url` varchar(200) DEFAULT NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('attachment', '0');

