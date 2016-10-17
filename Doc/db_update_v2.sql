DROP TABLE IF EXISTS `leave`;
CREATE TABLE `leave` (
  `id` int(11) NOT NULL,
  `owner_id` int(11) NULL,
  `type` int(11) DEFAULT NULL COMMENT '11-病假, 12-事假, 13-婚假, 14-丧假, 15-产假, 16-陪产假, 20-年假',
  `date_start` datetime DEFAULT NULL,
  `date_end` datetime DEFAULT NULL,
  `reason` varchar(200) DEFAULT NULL COMMENT '事由',
  `memo` varchar(200) DEFAULT NULL COMMENT '工作交接内容',
  `receiver_id` int(11) NULL COMMENT '工作交接人',
  `tel` varchar(20) DEFAULT NULL COMMENT '请假期间联系电话',
  `auditor_id` int(11) NULL COMMENT '审核人',
  `audit_memo` varchar(200) DEFAULT NULL COMMENT '驳回原因',
  `date_review` datetime DEFAULT NULL COMMENT '审核时间',
  `status` int(11) NULL COMMENT '状态 0-带审批，1：通过审批， 2：驳回，-1：作废',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('leave', '0');
