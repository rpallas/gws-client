using System;
using System.Collections;
using System.Collections.Generic;

public class Question
{
	public int Id { get; set; }
	public string QuestionText { get; set; }
	public List<Answer> Answers { get; set; }
	
	public Question ()
	{
	}
}

public class Answer
{
	public int Id { get;	set; }
	public string AnswerText { get;	set; }
	
	public Answer ()
	{		
	}	
}



