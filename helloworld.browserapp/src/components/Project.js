import React, { Component } from 'react';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';
import user from '../images/user.png';
import filledHeart from '../images/filled-heart.png';
import remove from '../images/delete.png';
import github from '../images/github.png';
import discord from '../images/discord.png';
import save from '../images/bookmark.png';
import saved from '../images/bookmarked.png';
import edit from '../images/edit.png';

import Tag from './Tag';
import ImageSlider from './ImageSlider';

import VisibilitySensor from 'react-visibility-sensor';

import { formatDate } from '../util';

import './Project.css';
import { sendJSONRequest } from '../requestFuncs';
import InputField from './InputField/InputField';


export class Project extends Component {
    state = {
        visibility: true
    }

    render() {
        let { id, title, description, tags, creatorImage, createdAt, creatorId, memberIds, imageUrls, usersLikedIds } = this.props.item;

        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)}>
                <div className="project-container" style={{
                    opacity: this.state.visibility ? 1 : 0.25,
                    transition: 'opacity 500ms linear',
                    width: this.props.width
                }} >
                    <div className="header-container">
                        <img src={creatorImage} alt="" height={30} width={30} />
                        <div className="header-middle">
                            <div className="header-text-container">
                                <p className="header-text">{title}</p>
                                {
                                    createdAt &&
                                    <p className="bold-gray-date header-date">{formatDate(createdAt)}</p>
                                }
                            </div>
                            <div className="header-tags">
                                {
                                    tags &&
                                    tags.map((item, index) =>
                                        <Tag key={index} margin="0px 5px" name={item} />
                                    )
                                }
                            </div>
                        </div>
                        <span className="header-actions">
                            <div className="header-action">
                                {
                                    this.props.previewMode &&
                                    <img src={save} width={30} height={30} alt="" />
                                }
                                {
                                    !this.props.previewMode &&
                                    <img src={this.props.saved ? saved : save} alt="" height={30} width={30} onClick={() => this.props.onSave(id)}/>
                                }
                            </div>
                            <div className="header-action">
                                <img src={user} width={30} height={30} alt="" />
                                <p className="header-likes">{memberIds?.length ?? 0}</p>
                            </div>
                            <div className="header-action" style={{ marginRight: '0px' }}>
                                {
                                    this.props.previewMode &&
                                    <img src={heart} width={30} height={30} alt="" />
                                }
                                {
                                    !this.props.previewMode &&
                                    <img src={this.props.usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart} width={30} height={30} alt=""
                                        onClick={() => this.props.onLike(this.props.keyProp)} />
                                }
                                <p className="header-likes">{usersLikedIds?.length ?? 0}</p>
                            </div>
                            {
                                this.props.previewMode &&
                                <img src={menuClosed} width={30} height={30} alt="" />
                            }
                            {
                                !this.props.previewMode &&
                                <DropDown toggleButton={{
                                    icon: undefined,
                                    arrowIconOpen: menuOpened,
                                    arrowIconClose: menuClosed
                                }}
                                    arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                    {
                                        this.props.sessionUserId === creatorId &&
                                        <DropDown.Item icon={remove} textColor="red" text="Delete" iconSize={30} onClick={() => this.props.onDelete(this.props.keyProp)} />
                                    }
                                    <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                    {
                                        this.props.sessionUserId === creatorId &&
                                        <DropDown.Item icon={edit} text="edit" iconSize={30} onClick={() => this.props.onEdit(this.props.keyProp)} />
                                    }
                                    <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                    <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                </DropDown>
                            }
                        </span>
                    </div>
                    <div className="description-container">
                        <p>{description}</p>
                    </div>
                    {
                        imageUrls &&
                        <ImageSlider images={imageUrls} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        )
    }
}


export class DetailedProject extends Component {
    state = {
        visibility: true,

        showMembers: false,
        members: [],
    }

    componentDidMount() {


        let members = [];
        for (var i = 0; i < this.props.item.memberIds.length; i++) {
            sendJSONRequest("GET", `/user/get_minimal/${this.props.item.memberIds[i]}`, undefined, this.props.tokens.token)
                .then(response => {
                    members = [...members, response.data]

                    if (i === this.props.memberIds.length) {
                        this.setState({ members: [...members] })
                    }
                }
            );
        }
    }

    renderLink = (item, index) => {
        let src = item.name === "Github" ? github : discord;
        return (
            <div key={index} className="center-vertical">
                <img src={src} alt="" height={25} width={25} />
                <a className="project-link" href={item.value} target="_blank" rel="noopener noreferrer">{item.value}</a>
            </div>
        )
    }

    render() {
        let { id, title, description, tags, creatorImage, createdAt, creatorId, memberIds, links, imageUrls, usersLikedIds } = this.props.item;

        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !creatorImage && this.props.onFirstAppear(this.props.keyProp)}>
                <div className="project-container" style={{
                        opacity: this.state.visibility ? 1 : 0.25,
                        transition: 'opacity 500ms linear',
                        width: this.props.width
                }} >
                    <div className="header-container">
                        <img src={creatorImage} alt="" height={30} width={30} />
                        <div className="header-middle">
                            <div className="header-text-container">
                                <p className="header-text">{title}</p>
                                {
                                    createdAt &&
                                    <p className="bold-gray-date header-date">{formatDate(createdAt)}</p>
                                }
                            </div>
                            <div className="header-tags">
                                {
                                    tags?.map((item, index) =>
                                        <Tag key={index} name={item} /> )
                                }
                            </div>
                        </div>
                        <span className="header-actions">
                            <div className="header-action" onClick={() => this.props.onSave(id)}>
                                <img src={this.props.saved ? saved : save} alt="" height={30} width={30}/>
                            </div>
                            <div className="header-action" onClick={() => this.setState({ showMembers: !this.state.showMembers })}>
                                <img src={user} width={30} height={30} alt="" />
                                <p className="header-likes">{memberIds?.length ?? 0}</p>
                                <div className="project-members-container">
                                {
                                    this.state.showMembers &&
                                    this.state.members.map((item, index) =>
                                        <InputField.IconTwoLineItem key={index} icon={item.imageUrl} header={item.userName}
                                            text={item.email} />)
                                }
                                </div>
                            </div>
                            <div className="header-action" style={{ marginRight: '0px' }}>
                                <img src={usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart} width={30} height={30} alt=""
                                    onClick={() => this.props.onLike(this.props.keyProp)} />
                                <p className="header-likes">{usersLikedIds?.length ?? 0}</p>
                            </div>
                            {
                                this.props.previewMode &&
                                <img src={menuClosed} width={30} height={30} alt="" />
                            }
                            {
                                !this.props.previewMode &&
                                <DropDown toggleButton={{
                                    icon: undefined,
                                    arrowIconOpen: menuOpened,
                                    arrowIconClose: menuClosed
                                }} arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                        {
                                            this.props.sessionUserId === creatorId &&
                                            <DropDown.Item icon={remove} textColor="red" text="Delete" iconSize={30} onClick={() => this.props.onDelete(this.props.keyProp)} />
                                        }
                                        <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                        {
                                            this.props.sessionUserId === creatorId &&
                                            <DropDown.Item icon={edit} text="edit" iconSize={30} onClick={() => this.props.onEdit(this.props.keyProp, this.props.item)} />
                                        }
                                        <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                        <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                </DropDown>
                            }

                        </span>
                    </div>
                    <div className="description-container">
                        <p>{description}</p>
                    </div>
                    {
                        links.length !== 0 && 
                        <div className="project-links-container">
                        {
                            links?.map((item, index) => this.renderLink(item, index))
                        }
                        </div>
                    }

                    {
                        imageUrls && imageUrls.length !== 0 &&
                        <ImageSlider images={imageUrls} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        )
    }
}
