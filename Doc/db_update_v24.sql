INSERT INTO `menu` VALUES ('78', '4', 'tax_warning', 'fa fa-calendar-check-o', '税表预警', '11');

DROP TABLE IF EXISTS `tax_record`;
CREATE TABLE `tax_record` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL COMMENT '订单ID',
  `deal_way` int(11) DEFAULT NULL COMMENT '处理方式，0未处理，1零申报，2转审计',
  `audit_id` int(11) NULL COMMENT '审计ID',
  `audit_code` varchar(20) DEFAULT NULL COMMENT '审计档案号',
  `sent_date` datetime DEFAULT NULL COMMENT '税表发出时间',

  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',
  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT 'url',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('tax_record', '1');

INSERT INTO `operation` VALUES ('7', '税表录入');

INSERT INTO `settings` VALUES ('16', 'TAX_ID', null, null);


-- 2018-08-12
call AddColumnUnlessExists(Database(), 'tax_record', 'end_date', 'datetime DEFAULT NULL COMMENT "税表截止日期"');

-- 2018-09-04
call AddColumnUnlessExists(Database(), 'customer', 'tag', 'varchar(120) DEFAULT NULL COMMENT "标签"');

call AddColumnUnlessExists(Database(), 'tax_record', 'end_date', 'datetime DEFAULT NULL COMMENT "税表截止日期"');

-- 2018-11-25
call AddColumnUnlessExists(Database(), 'reg_abroad', 'tax_type', 'int(11) DEFAULT NULL COMMENT "0-未定，1-零申报，2-转审计"');