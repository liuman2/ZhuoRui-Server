call AddColumnUnlessExists(Database(), 'reg_abroad', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE reg_abroad ADD CONSTRAINT fk_abroad_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'reg_internal', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE reg_internal ADD CONSTRAINT fk_internal_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'trademark', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE trademark ADD CONSTRAINT fk_trademark_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'patent', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE patent ADD CONSTRAINT fk_patent_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'audit', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE audit ADD CONSTRAINT fk_audit_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'annual_exam', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE annual_exam ADD CONSTRAINT fk_annual_exam_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'mail', 'audit_id', 'int(11) DEFAULT NULL COMMENT "审核人"');
ALTER TABLE mail ADD CONSTRAINT fk_mail_audit_id FOREIGN KEY (audit_id) REFERENCES member(id);
